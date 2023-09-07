using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using album_photo_web_api.Data.Services;
using album_photo_web_api.Data.ViewModels;
using album_photo_web_api.Models;
using album_photo_web_api.Data.Interfaces;
using Microsoft.AspNetCore.StaticFiles;

namespace album_photo_web_api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PhotoController : ControllerBase
    {
        public PhotoService _photoService;
        public static IWebHostEnvironment _webHostEnvironment;
        public PhotoController(PhotoService photoService, IWebHostEnvironment webHostEnvironment)
        {
            _photoService = photoService;
            _webHostEnvironment = webHostEnvironment;
        }

        [HttpGet("get-all-photos")] 
        public IActionResult GetPhotos()
        {
            var allPhotos = _photoService.GetAllPhotos();
            return Ok(allPhotos);
        }

        [HttpGet("get-photo-by-id/{photoId}")]
        public IActionResult GetPhotoById(int photoId)
        {
            var photo = _photoService.GetPhotoById(photoId);
            return Ok(photo);
        }

        [HttpPut("update-photo-by-id/{photoId}")] // 3. Edycja opisów (TAGI, SZCZEGÓŁY, APARAT) // 4. Zmiana status zdjęcia (PRIV/PUBLIC)
        public IActionResult UpdatePhotoById(int photoId, [FromForm] PhotoUpdateVM photo)
        {
            var photoUpd = _photoService.UpdatePhotoById(photoId, photo);
            return Ok(photoUpd);
        } 
        [HttpDelete("delete-photo-by-id/{photoId}")] // 2. Usuwanie zdjęć
        public IActionResult DeletePhotoById(int photoId)
        {
            _photoService.DeletePhotoById(photoId);
            return Ok();
        } 

        [HttpPost("add-photo")] // 1. Dodawanie zdjęć (TAG, AUTOR, SZCZEGÓŁY, APARAT, STATUS) // 1:4. Generowanie miniatur dla zdjęć
        public async Task<IActionResult> AddPhoto([FromForm] PhotoVM photo)
        {
            _photoService.AddPhoto(photo);
            return Ok(photo);
        }
        [HttpGet("show-photo-by-file-name")]
        public async Task<IActionResult> ShowPhotoByFileName([FromQuery] string fileName)
        {

            string path = _photoService.GetPathByFileName(fileName);
            if (System.IO.File.Exists(path))
            {
                byte[] b = System.IO.File.ReadAllBytes(path);
                return File(b, "image/jpg");
            }
            return null;
        }

        [HttpGet("download-photo-by-file-name")] // 6. Możliwość pobierania zdjęć zgodnie z prawami (publiczne, własne, admin)
        public async Task<IActionResult> DownloadPhoto(string fileName)
        {
            byte[] b = _photoService.DownloadPhoto(fileName);
            string path = _photoService.GetPathByFileName(fileName);
            return File(b, "application/octet-stream", Path.GetFileName(path));
        }
        [HttpGet("get-thumbnail-by-file-name")]
        public async Task<IActionResult> ShowThumbnailByFileName([FromQuery] string fileName)
        {

            string path = _photoService.GetPathByFileNameThumbnails(fileName);
            if (System.IO.File.Exists(path))
            {
                byte[] b = System.IO.File.ReadAllBytes(path);
                return File(b, "image/jpg");
            }
            return null;
        }
        [HttpPatch("change-photo-access-level")]
        public IActionResult ChangeAccess(int photoId)
        {
            return Ok(_photoService.ChangeAccessById(photoId));
        }
        [HttpGet("get_photo(s)-by-name")]
        public IActionResult GetPhotosByName(string photoName)
        {
            var filteredPhotos = _photoService.GetPhotosByName(photoName);
            return Ok(filteredPhotos);
        }

        // 5. Przeszukiwanie zdjęć na podstawie praw użytkownika (admin dostęp do wszystkiego)
        // 7. Zdjęcia można oceniać i dodać komentarz (oprócz własnych)

        
    }
}
