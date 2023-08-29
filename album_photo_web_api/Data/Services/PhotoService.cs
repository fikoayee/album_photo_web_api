using album_photo_web_api.Data.ViewModels;
using album_photo_web_api.Models;

namespace album_photo_web_api.Data.Services
{
    public class PhotoService
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
        public Photo UpdatePhotoById(int photoId, PhotoVM photo)
        {
            var photoUpd = _context.Photos.FirstOrDefault(c => c.Id == photoId);
            if (photoUpd != null)
            {
                photoUpd.Name = photo.Name;

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
    }
}
