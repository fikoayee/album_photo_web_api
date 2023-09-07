using album_photo_web_api.Data.Interfaces;
using album_photo_web_api.Data.ViewModels;
using album_photo_web_api.Helper;
using album_photo_web_api.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Hosting;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;

namespace album_photo_web_api.Data.Services
{
    public class PhotoService : IPhotoService
    {
        private AppDbContext _context;
        private IWebHostEnvironment _webHostEnvironment;
        public PhotoService(AppDbContext context, IWebHostEnvironment webHostEnvironment)
        {
            _context = context;
            _webHostEnvironment = webHostEnvironment;
        }

        public async void AddPhoto(PhotoVM photo)
        {
            string wwwRootPath = _webHostEnvironment.WebRootPath;
            string fileName = Path.GetFileName(photo.ImageFile.FileName);
            string extension = Path.GetExtension(photo.ImageFile.FileName);
            fileName = GetNextFileName(fileName, extension);

            var newPhoto = new Photo()
            {
                Name = photo.Name,
                Access = photo.Access,
                Camera = photo.Camera,
                Tags = photo.Tags.ToString(),
                ImageFile = photo.ImageFile,
                ImageName = fileName,
            };

            newPhoto.ImageName = fileName;
            string path = Path.Combine(wwwRootPath + @"\uploads\", fileName);
            string pathThumbnails = Path.Combine(wwwRootPath + @"\uploads\thumbnails", fileName);

            UploadPhoto(photo.ImageFile, path);

            ResizeImage(path, pathThumbnails);

            _context.Photos.Add(newPhoto);
            _context.SaveChanges();
        }
        public List<Photo> GetAllPhotos()
        {
            return _context.Photos.ToList();

        }
        public Photo GetPhotoById(int photoId)
        {
            return _context.Photos.FirstOrDefault(c => c.Id == photoId);
        }
        public Photo UpdatePhotoById(int photoId, PhotoUpdateVM photo)
        {
            var photoUpd = _context.Photos.FirstOrDefault(c => c.Id == photoId);
            if (photoUpd != null)
            {
                photoUpd.Name = photo.Name;
                photoUpd.Access = photo.Access;
                photoUpd.Camera = photo.Camera;
                photoUpd.Tags = photo.Tags.ToString();
                _context.SaveChanges();
            }
            return photoUpd;
        }
        public void DeletePhotoById(int photoId)
        {
            var photoDel = _context.Photos.FirstOrDefault(c => c.Id == photoId);
            if (photoDel != null)
            {
                _context.Photos.Remove(photoDel);
                _context.SaveChanges();
            }
        }
        public async Task UploadPhoto(IFormFile iFormFile, string path)
        {
            try
            {
                using (var _FileStream = new FileStream(path, FileMode.Create))
                {
                    await iFormFile.CopyToAsync(_FileStream);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public string GetPathByFileName(string fileName)
        {
            string wwwRootPath = _webHostEnvironment.WebRootPath;
            string path = Path.Combine(wwwRootPath + @"\uploads\", fileName);
            return path;
        }
        public string GetPathByFileNameThumbnails(string fileName)
        {
            string wwwRootPath = _webHostEnvironment.WebRootPath;
            string path = Path.Combine(wwwRootPath + @"\uploads\thumbnails", fileName);
            return path;
        }
        public byte[] DownloadPhoto(string fileName)

        {
            string path = GetPathByFileName(fileName);

            if (System.IO.File.Exists(path))
            {
                byte[] b = System.IO.File.ReadAllBytes(path);
                return b;
            }
            return null;
        }
        public void ResizeImage(string path, string pathThumbnails)
        {
            using (Image image = Image.FromFile(path))
            {
                var thumbnail = new Bitmap(image, 200, 200);
                thumbnail.Save(pathThumbnails, ImageFormat.Jpeg);
            }
        }
        public string GetNextFileName(string fileName, string extension)
        {

            int i = 0;
            while (File.Exists(fileName))
            {
                if (i == 0)
                    fileName = fileName.Replace(extension, "(" + ++i + ")" + extension);
                else
                    fileName = fileName.Replace("(" + i + ")" + extension, "(" + ++i + ")" + extension);
            }
            return fileName;
        }
        public string ChangeAccessById(int photoId)
        {
            var photo = GetPhotoById(photoId);
            if (photo.Access == AccessLevel.Public)
            {
                photo.Access = AccessLevel.Private;
                _context.SaveChanges();
                return "Changed access level to private";
            }
            else
            {
                photo.Access = AccessLevel.Public;
                _context.SaveChanges();
                return "Changed access level to public";
            }
        }

        public List<Photo> GetPhotosByAuthorName(string authorName)
        {
            throw new NotImplementedException();
        }

        public List<Photo> GetPhotosByAuthorId(int authorId)
        {
            throw new NotImplementedException();
        }

        public List<Photo> GetPhotosByName(string photoName)
        {
            var data = GetAllPhotos();
            var dataFiltered = data.Where(n => n.Name.IndexOf(photoName, StringComparison.OrdinalIgnoreCase) != -1);
            return dataFiltered.ToList();
        }
    }
}
