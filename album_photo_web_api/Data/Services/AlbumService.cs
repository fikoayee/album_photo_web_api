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
                //AuthorId = album.AuthorId,
                //Access = album.Access,
                //ImageName = album.ImageName,
                //PhotosId = album.PhotosId

            };
            _context.Albums.Add(newAlbum);
            _context.SaveChanges();

            //if (album.PhotosId != null)
            //{
            //    foreach (var photoId in album.PhotosId)
            //    {
            //        var newPhotoAlbum = new AlbumPhoto()
            //        {
            //            PhotoId = photoId,
            //            AlbumId = newAlbum.Id
            //        };
            //        _context.AlbumsPhotos.Add(newPhotoAlbum);
            //        _context.SaveChanges();
            //    }
            //}
        }

        public List<Album> GetAllAlbums()
        {
            return _context.Albums.ToList();

        }
        public Album GetAlbumById(int albumId)
        {
            return _context.Albums.FirstOrDefault(c => c.Id == albumId);
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

    }
}
