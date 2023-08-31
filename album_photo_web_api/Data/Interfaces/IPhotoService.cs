using album_photo_web_api.Data.ViewModels;
using album_photo_web_api.Models;

namespace album_photo_web_api.Data.Interfaces
{
    public interface IPhotoService
    {
        void AddPhoto(PhotoVM photo);
        List<Photo> GetAllPhotos(); 
        Photo GetPhotoById(int photoId); 
        Photo UpdatePhotoById(int photoId, PhotoUpdateVM photo);
        //Photo UpdateAccessByPhotoId(int photoId);
        void DeletePhotoById(int photoId);
        Task<string> UploadPhoto(IFormFile iFormFile);
        //Task<string> DownloadPhoot(Photo photo);

    }
}
