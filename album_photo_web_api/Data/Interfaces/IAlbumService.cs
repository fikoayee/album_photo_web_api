using album_photo_web_api.Data.ViewModels;
using album_photo_web_api.Dto;
using album_photo_web_api.Models;
using Microsoft.AspNetCore.Mvc;

namespace album_photo_web_api.Data.Interfaces
{
    public interface IAlbumService
    {
        public void AddAlbum(AlbumVM album, string userId, bool isAdmin);
        public List<AlbumDto> GetAllAlbums();
        public List<PhotoDto> GetAllPhotosFromAlbum(int albumId);
        public AlbumDto GetAlbumById(int albumId);
        public AlbumDto UpdateAlbumById(int albumId, [FromForm] AlbumUpdateVM album, string userId);
        public void DeleteAlbumById(int albumId);
        public AlbumPhoto GetAlbumPhotoByIds(int albumId, int photoId);
        public bool MovePhotos(bool isAdmin, string currentUserId, int currentAlbumId, int destinationAlbumId, List<int> photoIds);
        public bool AddPhotoByIds(bool isAdmin, string currentUserId, int albumId, List<int> photoIds);
        public bool RemovePhotoByIds(bool isAdmin, string currentUserId, int albumId, List<int> photoIds);
        public bool HasAccess(int albumId, string userId, bool isAdmin);
        public bool HasPriveleges(int albumId, string userId, bool isAdmin);
        public bool HasPriveleges(int albumId, int destAlbumId, string userId, bool isAdmin);
        public bool HasPrivlegesToPhoto(int photoId, string userId, bool isAdmin);
        public string GetUserIdByAlbumId(int albumId);
        public Album GetAlbumByIdPriv(int albumId);
        public bool AlbumExists(int albumId);
        public List<AlbumDto> GetAlbumsByName(string albumName);
        public List<AlbumDto> GetAlbumsByAuthorName(string authorName);
        public List<AlbumDto> GetAlbumsByAuthorId(string authorId);
        public string ChangeAccessById(int albumId);
        public string ChangeAccessByIdForAll(int albumId);
        public string DeleteAlbumWithPhotos(int albumId);













    }
}
