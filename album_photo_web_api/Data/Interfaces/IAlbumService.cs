using album_photo_web_api.Data.ViewModels;
using album_photo_web_api.Models;

namespace album_photo_web_api.Data.Interfaces
{
    public interface IAlbumService
    {
        void AddAlbum(AlbumVM album);
        List<Album> GetAllAlbums();
        Album GetAlbumById(int albumId);
        Album UpdateAlbumById(int albumId, AlbumVM album);
        void DeleteAlbumById(int albumId);
        AlbumPhoto GetAlbumPhotoByIds(int albumId, int photoId);
        Task<string> UploadPhoto(IFormFile iFormFile);
        void MovePhotos(int currentAlbumId, int destinationAlbumId, List<int> photoIds);
        void AddPhotoByIds(int albumId, List<int> photoIds);
        void RemovePhotoByIds(int albumId, List<int> photoIds);
    }
}
