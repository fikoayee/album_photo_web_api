using album_photo_web_api.Data.Interfaces;
using album_photo_web_api.Data.ViewModels;
using album_photo_web_api.Models;

namespace album_photo_web_api.Data.Services
{
    public class AlbumPhotoService : IAlbumPhotoService
    {
        private AppDbContext _context;
        public AlbumPhotoService(AppDbContext context)
        {
            _context = context;
        }

        public AlbumPhoto GetAlbumPhotoByIds(int albumId, int photoId)
        {
            var albumPhoto = _context.AlbumsPhotos.Where(p => p.PhotoId == photoId && p.AlbumId == albumId).FirstOrDefault();
            return albumPhoto;
        }
    }
}
