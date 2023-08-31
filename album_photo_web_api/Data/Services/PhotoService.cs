using album_photo_web_api.Data.Interfaces;
using album_photo_web_api.Data.ViewModels;
using album_photo_web_api.Helper;
using album_photo_web_api.Models;

namespace album_photo_web_api.Data.Services
{
    public class PhotoService :IPhotoService
    {
        private AppDbContext _context;
        public PhotoService(AppDbContext context)
        {
            _context = context;
        }

        public void AddPhoto(PhotoVM photo)
        {
            var newPhoto = new Photo()
            {
                Name = photo.Name,
                Access = photo.Access,
                Camera = photo.Camera,
                Tags = photo.Tags.ToString(),
                
            };
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

        public async Task<string> UploadPhoto(IFormFile iFormFile)
        {
            string fileName = "";
            try
            {
                FileInfo _FileInfo = new FileInfo(iFormFile.FileName);
                fileName = iFormFile.FileName + _FileInfo.Extension;
                var getFilePath = CommonPath.GetFilePath(fileName);
                using (var _FileStream = new FileStream(getFilePath, FileMode.Create))
                {
                    await iFormFile.CopyToAsync(_FileStream);
                }
                return fileName;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
