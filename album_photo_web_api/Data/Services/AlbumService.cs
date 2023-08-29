using album_photo_web_api.Data.ViewModels;
using album_photo_web_api.Models;

namespace album_photo_web_api.Data.Services
{
    public class AlbumService
    {
        private AppDbContext _context;
        public AlbumService(AppDbContext context)
        {
            _context = context;
        }

        public void AddAlbum(AlbumVM album)
        {
            var newAlbum = new Album()
            {
                Name = album.Name,
            };
            _context.Albums.Add(newAlbum);
            _context.SaveChanges();

            if (album.PhotoId != null)
            {
                foreach (var photoId in album.PhotoId)
                {
                    var newPhotoAlbum = new AlbumPhoto()
                    {
                        PhotoId = photoId,
                        AlbumId = newAlbum.Id
                    };
                    _context.AlbumsPhotos.Add(newPhotoAlbum);
                    _context.SaveChanges();
                }
            }
        }

        public List<Album> GetAllAlbums()
        {
            return _context.Albums.ToList();

        }
        public AlbumWithPhotosVM GetAlbumById(int albumId)
        {
            var albumWithPhotos = _context.Albums.Where(p => p.Id == albumId).Select(album => new AlbumWithPhotosVM()
            {
                Name = album.Name,
                PhotoNames = album.AlbumsPhotos.Select(p => p.Photo.Name).ToList()
            }).FirstOrDefault();

            return albumWithPhotos;
        }
        public Album UpdateAlbumById(int albumId, AlbumVM album)
        {
            var albumUpd = _context.Albums.FirstOrDefault(c => c.Id == albumId);
            if (albumUpd != null)
            {
                albumUpd.Name = album.Name;

                _context.SaveChanges();
            }
            return albumUpd;
        }
        public void DeleteAlbumById(int albumId)
        {
            var albumDel = _context.Albums.FirstOrDefault(c => c.Id == albumId);
            if (albumDel != null)
            {
                _context.Albums.Remove(albumDel);
                _context.SaveChanges();
            }
        }

        /////////////
        ///
        public void MovePhotos(int currentAlbumId, List<int> photoIds, int destinationAlbumId)
        {
            var albumCurr = _context.Albums.FirstOrDefault(a => a.Id == currentAlbumId);

            foreach (var photoId in photoIds)
            {

                var deleteAlbumPhoto = GetAlbumPhotoByIds(currentAlbumId, photoId);

                var newAlbumPhoto = new AlbumPhoto()
                {
                    AlbumId = destinationAlbumId,
                    PhotoId = photoId,
                };
                _context.AlbumsPhotos.Add(newAlbumPhoto);
                _context.AlbumsPhotos.Remove(deleteAlbumPhoto);
                _context.SaveChanges();
            }
        }
        public AlbumPhoto GetAlbumPhotoByIds(int albumId, int photoId)
        {
            var albumPhoto = _context.AlbumsPhotos.Where(p => p.PhotoId == photoId && p.AlbumId == albumId).FirstOrDefault();
            return albumPhoto;
        }
        public void RemovePhotoByIds(int albumId, List<int> photoIds)
        {
            foreach (var photoId in photoIds)
            {
                var deleteAlbumPhoto = GetAlbumPhotoByIds(albumId, photoId);
                _context.AlbumsPhotos.Remove(deleteAlbumPhoto);
                _context.SaveChanges();
            }
        }
        public void AddPhotoByIds(int albumId, List<int> photoIds)
        {
            foreach (var photoId in photoIds)
            {
                var newAlbumphoto = new AlbumPhoto()
                {
                    AlbumId = albumId,
                    PhotoId = photoId,
                };
                _context.Add(newAlbumphoto);
                _context.SaveChanges();
            }
        }
    }
}
