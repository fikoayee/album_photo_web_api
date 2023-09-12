using album_photo_web_api.Data.ViewModels;
using album_photo_web_api.Dto;
using album_photo_web_api.Models;
using Microsoft.AspNetCore.Mvc;

namespace album_photo_web_api.Data.Interfaces
{
    public interface IPhotoService
    {
        void AddPhoto(PhotoVM photo, string userId);
        public bool PhotoExists(int photoId);
        public Photo GetPhotoByIdPriv(int photoId);
        public List<PhotoDto> GetAllPhotos();
        public PhotoDto GetPhotoById(int photoId);
        public PhotoDto UpdatePhotoById(int photoId, PhotoUpdateVM photo, string userId);
        public void DeletePhotoById(int photoId);
        public async Task UploadPhoto(IFormFile iFormFile, string path);
        public string GetPathByFileName(string fileName);
        public string GetPathByFileNameThumbnails(string fileName);
        public byte[] DownloadPhoto(string fileName);
        public void ResizeImage(string path, string pathThumbnails);
        public string GetNextFileName(string fileName, string extension);
        public string ChangeAccessById(int photoId);
        public List<PhotoDto> GetPhotosByAuthorName(string authorName);
        public List<PhotoDto> GetPhotosByAuthorId(string authorId);
        public List<PhotoDto> GetPhotosByName(string photoName);
        public Photo GetPhotoByFileName(string fileName);
        public bool HasAccess(int photoId, string userId, bool isAdmin);
        public bool HasPriveleges(int photoId, string userId, bool isAdmin);
        public string GetUserIdByPhotoId(int photoId);
        public void AddUpvote(int photoId);
        public void AddDownvote(int photoId);
        public bool GetVoteDetails(string userId, int photoId);
    }
}
