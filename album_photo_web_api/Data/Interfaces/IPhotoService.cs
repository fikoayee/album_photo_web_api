using album_photo_web_api.Data.ViewModels;
using album_photo_web_api.Dto;
using album_photo_web_api.Models;
using Microsoft.AspNetCore.Mvc;

namespace album_photo_web_api.Data.Interfaces
{
    public interface IPhotoService
    {
        void AddPhoto(PhotoVM photo, string userId);
        //List<PhotoDto> GetAllPhotos(); 
        PhotoDto GetPhotoById(int photoId);
        PhotoDto UpdatePhotoById(int photoId, PhotoUpdateVM photo, string userId);
        void DeletePhotoById(int photoId);
        Task UploadPhoto(IFormFile iFormFile, string path);
        public string GetPathByFileNameThumbnails(string fileName);
        public byte[] DownloadPhoto(string fileName);
        public void ResizeImage(string path, string pathThumbnails);
        public string GetNextFileName(string fileName, string extension);
        string ChangeAccessById(int photoId);
        public List<PhotoDto> GetPhotosByAuthorName(string authorName);
        public List<PhotoDto> GetPhotosByAuthorId(string authorId);
        public List<PhotoDto> GetPhotosByName(string photoName);
        public Photo GetPhotoByFileName(string fileName);
        public bool HasAccess(int photoId, string userId, bool isAdmin);
        public bool HasPriveleges(int photoId, string userId, bool isAdmin);
        public string GetUserIdByPhotoId(int photoId);
    }
}
