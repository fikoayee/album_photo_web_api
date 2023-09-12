using album_photo_web_api.Data.Interfaces;
using album_photo_web_api.Data.ViewModels;
using album_photo_web_api.Dto;
using album_photo_web_api.Helper;
using album_photo_web_api.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Security.Claims;

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

        public async void AddPhoto(PhotoVM photo, string userId)
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
                UserId = userId,

            };

            newPhoto.ImageName = fileName;
            string path = Path.Combine(wwwRootPath + @"\uploads\", fileName);
            string pathThumbnails = Path.Combine(wwwRootPath + @"\uploads\thumbnails", fileName);

            UploadPhoto(photo.ImageFile, path);

            ResizeImage(path, pathThumbnails);

            _context.Photos.Add(newPhoto);
            _context.SaveChanges();
        }
        public bool PhotoExists(int photoId)
        {
            var photo = _context.Photos.FirstOrDefault(c => c.Id == photoId);
            if (photo == null)
                return false;
            else
                return true;
        }
        public Photo GetPhotoByIdPriv(int photoId)
        {
            if (PhotoExists(photoId))
                return _context.Photos.FirstOrDefault(c => c.Id == photoId);
            else
                return null;
        }
        public List<PhotoDto> GetAllPhotos()
        {
            var photos = new List<PhotoDto>();
            foreach (var p in _context.Photos.Include(u => u.User).Include(c => c.Comments).ToList())
            {
                PhotoDto obj = new PhotoDto()
                {
                    Id = p.Id,
                    Name = p.Name,
                    //Tags = p.Tags,
                    Camera = p.Camera,
                    Access = p.Access,
                    ImageName = p.ImageName,
                    UpVotes = p.UpVotes,
                    DownVotes = p.DownVotes,
                    Comments = p.Comments,
                    User = p.User.UserName,
                    UserId = p.UserId,
                };
                photos.Add(obj);
            }
            return (photos);
        }
        public PhotoDto GetPhotoById(int photoId)
        {
            var photo = _context.Photos.Include(u => u.User).Include(c => c.Comments).FirstOrDefault(c => c.Id == photoId);
            if (photo == null)
                return null;
            else
            {
                PhotoDto obj = new PhotoDto()
                {
                    Id = photo.Id,
                    Name = photo.Name,
                    //Tags = photo.Tags,
                    Camera = photo.Camera,
                    Access = photo.Access,
                    ImageName = photo.ImageName,
                    UpVotes = photo.UpVotes,
                    DownVotes = photo.DownVotes,
                    Comments = photo.Comments,
                    User = photo.User.UserName,
                    UserId = photo.UserId,
                };
                return (obj);
            }

        }
        public PhotoDto UpdatePhotoById(int photoId, PhotoUpdateVM photo, string userId)
        {
            var photoUpd = _context.Photos.FirstOrDefault(c => c.Id == photoId);
            if (photoUpd != null)
            {
                photoUpd.Name = photo.Name;
                photoUpd.Access = photo.Access;
                photoUpd.Camera = photo.Camera;
                photoUpd.UserId = userId;
                photoUpd.Tags = photo.Tags.ToString();

                _context.SaveChanges();
            }

            var obj = GetPhotoById(photoId);

            return obj;
        }
        public void DeletePhotoById(int photoId)
        {
            var photoDel = _context.Photos.Include(c => c.Comments).FirstOrDefault(c => c.Id == photoId);
            if (photoDel != null)
            {
                foreach (var comment in photoDel.Comments)
                {
                    _context.Comments.Remove(comment);
                }
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
            string wwwRootPath = _webHostEnvironment.WebRootPath;
            string path = Path.Combine(wwwRootPath + @"\uploads\", fileName);

            int i = 0;
            while (File.Exists(path))
            {
                if (i == 0)
                    fileName = fileName.Replace(extension, "(" + ++i + ")" + extension);
                else
                    fileName = fileName.Replace("(" + i + ")" + extension, "(" + ++i + ")" + extension);
                path = Path.Combine(wwwRootPath + @"\uploads\", fileName);
            }
            return fileName;
        }
        public string ChangeAccessById(int photoId)
        {
            var photo = GetPhotoByIdPriv(photoId);

            if (photo == null)
                return null;
            else if (photo.Access == AccessLevel.Public)
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
        public List<PhotoDto> GetPhotosByAuthorName(string authorName)
        {
            var user = _context.Users.FirstOrDefault(u => u.UserName == authorName);

            if (user == null)
                return null;
            else
                return GetAllPhotos().Where(p => p.UserId == user.Id).ToList();
        }
        public List<PhotoDto> GetPhotosByAuthorId(string authorId)
        {
            var data = GetAllPhotos().Where(p => p.UserId == authorId).ToList();
            return data;
        }
        public List<PhotoDto> GetPhotosByName(string photoName)
        {
            var data = GetAllPhotos();
            var dataFiltered = data.Where(n => n.Name.IndexOf(photoName, StringComparison.OrdinalIgnoreCase) != -1);
            return dataFiltered.ToList();

        }
        public Photo GetPhotoByFileName(string fileName)
        {
            return _context.Photos.FirstOrDefault(c => c.ImageName == fileName);
        }
        public bool HasAccess(int photoId, string userId, bool isAdmin)
        {
            var photo = GetPhotoById(photoId);

            if (isAdmin || (photo.Access == AccessLevel.Private && userId == photo.UserId) || photo.Access == AccessLevel.Public)
                return true;
            else
                return false;
        }
        public bool HasPriveleges(int photoId, string userId, bool isAdmin)
        {
            var photo = GetPhotoById(photoId);

            if (isAdmin || userId == photo.UserId)
                return true;
            else
                return false;
        }
        public string GetUserIdByPhotoId(int photoId)
        {
            var photo = GetPhotoByIdPriv(photoId);
            if (photo == null)
                return null;
            else
                return photo.UserId;
        }
        public void AddUpvote(int photoId)
        {
            var photo = GetPhotoByIdPriv(photoId);
            photo.UpVotes++;
            _context.SaveChanges();
            int num = photo.UpVotes;
            num = photo.UpVotes;
        }
        public void AddDownvote(int photoId)
        {
            var photo = GetPhotoByIdPriv(photoId);
            photo.DownVotes++;
            _context.SaveChanges();
            int num = photo.DownVotes;
            num = photo.DownVotes;
        }
        public bool GetVoteDetails(string userId, int photoId)
        {
            var vote = _context.Rates.Where(x => x.AuthorId == userId && x.PhotoId == photoId).FirstOrDefault();
            if (vote == null && PhotoExists(photoId) == true)
            {
                var newVote = new Rate()
                {
                    AuthorId = userId,
                    PhotoId = photoId,
                    IsRated = true,
                };
                _context.Rates.Add(newVote);
                _context.SaveChanges();
                return true;
            }
            else
                return false;

        }




    }
}
