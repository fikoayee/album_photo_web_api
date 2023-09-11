using album_photo_web_api.Data.ViewModels;
using album_photo_web_api.Models;

using album_photo_web_api.Helper;
using Microsoft.AspNetCore.Mvc;
using album_photo_web_api.Dto;
using Microsoft.EntityFrameworkCore;

namespace album_photo_web_api.Data.Services
{
    public class AlbumService
    {
        private AppDbContext _context;
        public AlbumService(AppDbContext context)
        {
            _context = context;
        }

        public void AddAlbum(AlbumVM album, string userId, bool isAdmin)
        {
            var newAlbum = new Album()
            {
                Name = album.Name,
                Access = album.Access,
                UserId = userId,
            };
            _context.Albums.Add(newAlbum);
            _context.SaveChanges();

            if (album.PhotoId != null)
            {
                foreach (var photoId in album.PhotoId)
                {
                    if (HasPrivlegesToPhoto(photoId, userId, isAdmin))
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
        }
        public List<AlbumDto> GetAllAlbums()
        {
            var albums = new List<AlbumDto>();
            foreach (var a in _context.Albums.Include(u => u.User).ToList())
            {
                AlbumDto obj = new AlbumDto()
                {
                    Id = a.Id,
                    Name = a.Name,
                    Access = a.Access,
                    User = a.User.UserName,
                    UserId = a.UserId,
                    Photos = GetAllPhotosFromAlbum(a.Id),
                };
                albums.Add(obj);
            }
            return (albums);

        }

        public List<PhotoDto> GetAllPhotosFromAlbum(int albumId)
        {
            var photos = new List<PhotoDto>();
            foreach (var albumPhoto in _context.AlbumsPhotos.Where(x => x.AlbumId == albumId).Include(p => p.Photo).Include(u => u.Photo.User))
            {
                PhotoDto obj = new PhotoDto()
                {
                    Id = albumPhoto.Photo.Id,
                    Name = albumPhoto.Photo.Name,
                    //Tags = p.Tags,
                    Camera = albumPhoto.Photo.Camera,
                    Access = albumPhoto.Photo.Access,
                    ImageName = albumPhoto.Photo.ImageName,
                    UpVotes = albumPhoto.Photo.UpVotes,
                    DownVotes = albumPhoto.Photo.DownVotes,
                    Comments = albumPhoto.Photo.Comments,
                    User = albumPhoto.Photo.User.UserName,
                    UserId = albumPhoto.Photo.UserId,
                };
                photos.Add(obj);
            }
            return (photos);
        }
        public AlbumDto GetAlbumById(int albumId)
        {
            var album = _context.Albums.Include(u => u.User).FirstOrDefault(p => p.Id == albumId);
            if (album == null)
                return null;
            else
            {
                AlbumDto obj = new AlbumDto()
                {
                    Id = albumId,
                    Name = album.Name,
                    Access = album.Access,
                    UserId = album.UserId,
                    User = album.User.UserName,
                    Photos = GetAllPhotosFromAlbum(albumId),
                };
                return (obj);
            }
        }
        public AlbumDto UpdateAlbumById(int albumId, [FromForm] AlbumUpdateVM album, string userId)
        {
            var albumUpd = _context.Albums.FirstOrDefault(c => c.Id == albumId);
            if (albumUpd != null)
            {
                albumUpd.Name = album.Name;
                albumUpd.Access = album.Access;
                _context.SaveChanges();
            }
            var obj = GetAlbumById(albumId);
            return obj;
        }
        public void DeleteAlbumById(int albumId)
        {
            var albumDel = _context.Albums.FirstOrDefault(c => c.Id == albumId);
            var albumPhotoDel = _context.AlbumsPhotos.Where(a => a.AlbumId == albumId);
            if (albumDel != null)
            {
                foreach (var albumPhoto in albumPhotoDel)
                {
                    _context.AlbumsPhotos.Remove(albumPhoto);

                }
                _context.Remove(albumDel);
                _context.SaveChanges();
            }
        }
        public AlbumPhoto GetAlbumPhotoByIds(int albumId, int photoId)
        {
            var albumPhoto = _context.AlbumsPhotos.Where(p => p.PhotoId == photoId && p.AlbumId == albumId).FirstOrDefault();
            if (albumPhoto != null)
                return albumPhoto;
            else
                return null;
        }
        public bool MovePhotos(bool isAdmin, string currentUserId, int currentAlbumId, int destinationAlbumId, List<int> photoIds)
        {
            bool stateChanged = false;
            if (HasPriveleges(currentAlbumId, destinationAlbumId, currentUserId, isAdmin))
            {
                foreach (var photoId in photoIds)
                {
                    if (HasPrivlegesToPhoto(photoId, currentUserId, isAdmin) && GetAlbumPhotoByIds(currentAlbumId, photoId) != null)
                    {
                        var updateAlbumPhoto = GetAlbumPhotoByIds(currentAlbumId, photoId);
                        updateAlbumPhoto.AlbumId = destinationAlbumId;
                        _context.AlbumsPhotos.Update(updateAlbumPhoto);
                        stateChanged = true;
                    }
                    _context.SaveChanges();
                }
            }
            return stateChanged;
        }
        public bool AddPhotoByIds(bool isAdmin, string currentUserId, int albumId, List<int> photoIds)
        {
            bool stateChanged = false;
            if (HasPriveleges(albumId, currentUserId, isAdmin))
            {
                foreach (var photoId in photoIds)
                {
                    if (HasPrivlegesToPhoto(photoId, currentUserId, isAdmin))
                    {
                        var newAlbumphoto = new AlbumPhoto()
                        {
                            AlbumId = albumId,
                            PhotoId = photoId,
                        };
                        stateChanged = true;
                        _context.Add(newAlbumphoto);
                        _context.SaveChanges();
                    }
                }
            }
            return stateChanged;
        }
        public bool RemovePhotoByIds(bool isAdmin, string currentUserId, int albumId, List<int> photoIds)
        {
            bool stateChanged = false;
            if (HasPriveleges(albumId, currentUserId, isAdmin))
            {
                foreach (var photoId in photoIds)
                {
                    if (HasPrivlegesToPhoto(photoId, currentUserId, isAdmin) && GetAlbumPhotoByIds(albumId, photoId) != null)
                    {
                        var deleteAlbumPhoto = GetAlbumPhotoByIds(albumId, photoId);
                        _context.AlbumsPhotos.Remove(deleteAlbumPhoto);
                        _context.SaveChanges();
                        stateChanged = true;
                    }
                }
            }
            return stateChanged;
        }
        public bool HasAccess(int albumId, string userId, bool isAdmin)
        {
            var album = GetAlbumById(albumId);

            if (isAdmin || (album.Access == AccessLevel.Private && userId == album.UserId) || album.Access == AccessLevel.Public)
                return true;
            else
                return false;
        }
        public bool HasPriveleges(int albumId, string userId, bool isAdmin)
        {
            var album = GetAlbumById(albumId);

            if (album == null)
                return false;
            else if (isAdmin || userId == album.UserId)
                return true;
            else
                return false;
        }
        public bool HasPriveleges(int albumId, int destAlbumId, string userId, bool isAdmin)
        {
            var album = GetAlbumById(albumId);
            var destAlbum = GetAlbumById(destAlbumId);

            if (album == null || destAlbum == null)
                return false;
            else if (isAdmin || (userId == album.UserId && userId == destAlbum.UserId))
                return true;
            else
                return false;
        }
        public bool HasPrivlegesToPhoto(int photoId, string userId, bool isAdmin)
        {
            var photo = _context.Photos.FirstOrDefault(p => p.Id == photoId);

            if (photo == null)
                return false;
            else if (isAdmin || userId == photo.UserId)
                return true;
            else
                return false;
        }
        public string GetUserIdByAlbumId(int albumId)
        {
            var album = GetAlbumByIdPriv(albumId);
            if (album == null)
                return null;
            else
                return album.UserId;
        }
        private Album GetAlbumByIdPriv(int albumId)
        {
            if (AlbumExists(albumId))
                return _context.Albums.FirstOrDefault(c => c.Id == albumId);
            else
                return null;
        }
        public bool AlbumExists(int albumId)
        {
            var album = _context.Albums.FirstOrDefault(c => c.Id == albumId);
            if (album == null)
                return false;
            else
                return true;
        }
        public List<AlbumDto> GetAlbumsByName(string albumName)
        {
            var data = GetAllAlbums();
            var dataFiltered = data.Where(n => n.Name.IndexOf(albumName, StringComparison.OrdinalIgnoreCase) != -1);
            return dataFiltered.ToList();
        }
        public List<AlbumDto> GetAlbumsByAuthorName(string authorName)
        {
            var user = _context.Users.FirstOrDefault(u => u.UserName == authorName);

            if (user == null)
                return null;
            else
                return GetAllAlbums().Where(p => p.UserId == user.Id).ToList();
        }
        public List<AlbumDto> GetAlbumsByAuthorId(string authorId)
        {
            var data = GetAllAlbums().Where(p => p.UserId == authorId).ToList();
            return data;
        }
        public string ChangeAccessById(int albumId)
        {
            var album = GetAlbumByIdPriv(albumId);

            if (album == null)
                return null;
            else if (album.Access == AccessLevel.Public)
            {
                album.Access = AccessLevel.Private;
                _context.SaveChanges();
                return "Changed access level to private";
            }
            else
            {
                album.Access = AccessLevel.Public;
                _context.SaveChanges();
                return "Changed access level to public";
            }
        }
        public string ChangeAccessByIdForAll(int albumId)
        {
            var album = GetAlbumByIdPriv(albumId);
            List<Photo> photos = _context.AlbumsPhotos.Where(a => a.AlbumId == albumId).Select(c => c.Photo).ToList();

            if (album == null)
                return null;
            else if (album.Access == AccessLevel.Public)
            {
                foreach (var photo in photos)
                {
                    photo.Access = AccessLevel.Private;
                }
                album.Access = AccessLevel.Private;
                _context.SaveChanges();
                return "Changed access level to private";
            }
            else
            {
                foreach (var photo in photos)
                {
                    photo.Access = AccessLevel.Public;
                }
                album.Access = AccessLevel.Public;
                _context.SaveChanges();
                return "Changed access level to public";
            }
        }
        public string DeleteAlbumWithPhotos(int albumId)
        {
            var album = GetAlbumByIdPriv(albumId);
            string stateChanged = "Could not delete!";
            if (album != null)
            {
                List<Photo> photos = _context.AlbumsPhotos.Where(a => a.AlbumId == albumId).Select(c => c.Photo).ToList();
                List<AlbumPhoto> albumsPhotos = new List<AlbumPhoto>();
                foreach (var photo in photos)
                {
                    var ap = GetAlbumPhotoByIds(albumId, photo.Id);
                    _context.AlbumsPhotos.Remove(ap);
                    _context.Photos.Remove(photo);
                }
                _context.Albums.Remove(album);
                _context.SaveChanges();
                stateChanged = "Deleted successfully!";
            }
            return stateChanged;
        }
    }
}
