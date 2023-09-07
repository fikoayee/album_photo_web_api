using album_photo_web_api.Data.ViewModels;
using album_photo_web_api.Models;
using Microsoft.AspNetCore.Mvc;

namespace album_photo_web_api.Data.Interfaces
{
    public interface IPhotoService
    {
        void AddPhoto(PhotoVM photo);
        List<Photo> GetAllPhotos(); 
        Photo GetPhotoById(int photoId); 
        Photo UpdatePhotoById(int photoId, PhotoUpdateVM photo);
        void DeletePhotoById(int photoId);
        Task UploadPhoto(IFormFile iFormFile, string path);
        public string GetPathByFileNameThumbnails(string fileName);
        public byte[] DownloadPhoto(string fileName);
        public void ResizeImage(string path, string pathThumbnails);
        public string GetNextFileName(string fileName, string extension);
        string ChangeAccessById(int photoId);
        public List<Photo> GetPhotosByAuthorName(string authorName);
        public List<Photo> GetPhotosByAuthorId(int authorId);
        public List<Photo> GetPhotosByName(string photoName);




    }
}
