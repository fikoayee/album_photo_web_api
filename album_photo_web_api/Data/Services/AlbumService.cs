using album_photo_web_api.Data.ViewModels;
using album_photo_web_api.Models;
using static Azure.Core.HttpHeader;
using album_photo_web_api.Helper;
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
                Access = album.Access,
                ImageFile = album.ImageFile,
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
        public Album GetAlbumById(int albumId)
        {
            return _context.Albums.FirstOrDefault(p => p.Id == albumId);
        }
        public Album UpdateAlbumById(int albumId, AlbumVM album)
        {
            var albumUpd = _context.Albums.FirstOrDefault(c => c.Id == albumId);
            if (albumUpd != null)
            {
                albumUpd.Name = album.Name;
                albumUpd.Access = album.Access;
                albumUpd.ImageFile = album.ImageFile;
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

        public AlbumPhoto GetAlbumPhotoByIds(int albumId, int photoId)
        {
            var albumPhoto = _context.AlbumsPhotos.Where(p => p.PhotoId == photoId && p.AlbumId == albumId).FirstOrDefault();
            return albumPhoto;
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
        public void MovePhotos(int currentAlbumId, int destinationAlbumId, List<int> photoIds)
        {
            foreach (var photoId in photoIds)
            {
                var updateAlbumPhoto = GetAlbumPhotoByIds(currentAlbumId, photoId);

                updateAlbumPhoto.AlbumId = destinationAlbumId;
                _context.AlbumsPhotos.Update(updateAlbumPhoto);
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
        public void RemovePhotoByIds(int albumId, List<int> photoIds)
        {
            foreach (var photoId in photoIds)
            {
                var deleteAlbumPhoto = GetAlbumPhotoByIds(albumId, photoId);
                _context.AlbumsPhotos.Remove(deleteAlbumPhoto);
                _context.SaveChanges();
            }
        }

    }
}
