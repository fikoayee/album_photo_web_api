using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using album_photo_web_api.Data.Services;
using album_photo_web_api.Data.ViewModels;
using album_photo_web_api.Models;
using album_photo_web_api.Data.Interfaces;
using Microsoft.AspNetCore.StaticFiles;
using System.Security.Claims;
using album_photo_web_api.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using System.Net.NetworkInformation;
using Microsoft.AspNetCore.Authorization;
using System.Data;

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
        public IActionResult GetPhotos()         // 5. Przeszukiwanie zdjęć na podstawie praw użytkownika (admin dostęp do wszystkiego)
        {
            var allPhotos = _photoService.GetAllPhotos();
            bool isAdmin = User.IsInRole("ADMIN");
            string userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (isAdmin)
                allPhotos = allPhotos;
            else
                allPhotos = allPhotos.Where(p => p.Access == Data.AccessLevel.Public || p.UserId == userId).ToList();

            return Ok(allPhotos);
        }

        [HttpGet("get-photo-by-id/{photoId}")]
        public IActionResult GetPhotoById(int photoId)
        {
            var photo = _photoService.GetPhotoById(photoId);
            if (photo == null)
                return NotFound();
            else
            {
                string userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                bool isAdmin = User.IsInRole("ADMIN");

                if (!_photoService.HasAccess(photoId, userId, isAdmin))
                    return Forbid();
                return Ok(photo);
            }
           
        }

        [Authorize(Roles = UserRoles.User + "," + UserRoles.Admin)]
        [HttpPut("update-photo-by-id/{photoId}")] // 3. Edycja opisów (TAGI, SZCZEGÓŁY, APARAT) // 4. Zmiana status zdjęcia (PRIV/PUBLIC)
        public IActionResult UpdatePhotoById(int photoId, [FromForm] PhotoUpdateVM photo)
        {
            string userId = _photoService.GetUserIdByPhotoId(photoId);
            bool isAdmin = User.IsInRole("ADMIN");
            string currUserId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (!_photoService.PhotoExists(photoId))
            {
                return NotFound();
            }
            else
            {
                if (_photoService.HasPriveleges(photoId, currUserId, isAdmin))
                {
                    var photoUpd = _photoService.UpdatePhotoById(photoId, photo, userId);
                    return Ok(photoUpd); ;
                }
                else
                    return Forbid();
            }
        }
        
        [Authorize(Roles = UserRoles.User + "," + UserRoles.Admin)]
        [HttpDelete("delete-photo-by-id/{photoId}")] // 2. Usuwanie zdjęć
        public IActionResult DeletePhotoById(int photoId)
        {
            bool isAdmin = User.IsInRole("ADMIN");
            string currUserId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if(!_photoService.PhotoExists(photoId))
            {
                return NotFound();
            }
            else
            {
                if (_photoService.HasPriveleges(photoId, currUserId, isAdmin))
                {
                    _photoService.DeletePhotoById(photoId);
                    return Ok();
                }
                else
                    return Forbid();
            }
            

            
        }

        [Authorize(Roles = UserRoles.User + "," + UserRoles.Admin)]
        [HttpPost("add-photo")] // 1. Dodawanie zdjęć (TAG, AUTOR, SZCZEGÓŁY, APARAT, STATUS) // 1:4. Generowanie miniatur dla zdjęć
        public async Task<IActionResult> AddPhoto([FromForm] PhotoVM photo)
        {
            string userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            _photoService.AddPhoto(photo, userId);
            return Ok(photo);
        }
        [HttpGet("show-photo-by-file-name")]
        public async Task<IActionResult> ShowPhotoByFileName([FromQuery] string fileName)
        {
            string userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            bool isAdmin = User.IsInRole("ADMIN");
            var photo = _photoService.GetPhotoByFileName(fileName);
            string path = _photoService.GetPathByFileName(fileName);

            if (!System.IO.File.Exists(path))
                return NotFound();
            else if (!_photoService.HasAccess(photo.Id, userId, isAdmin))
                return Forbid();
            else
            {
                byte[] b = System.IO.File.ReadAllBytes(path);
                return File(b, "image/jpg");
            }
        }

        [HttpGet("download-photo-by-file-name")] // 6. Możliwość pobierania zdjęć zgodnie z prawami (publiczne, własne, admin)
        public async Task<IActionResult> DownloadPhoto(string fileName)
        {
            string userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            bool isAdmin = User.IsInRole("ADMIN");
            var photo = _photoService.GetPhotoByFileName(fileName);
            string path = _photoService.GetPathByFileName(fileName);

            if (!System.IO.File.Exists(path))
                return NotFound();
            else if (!_photoService.HasAccess(photo.Id, userId, isAdmin))
                return Forbid();
            else
            {
                byte[] b = _photoService.DownloadPhoto(fileName);
                return File(b, "application/octet-stream", Path.GetFileName(path));
            }
        }
        [HttpGet("get-thumbnail-by-file-name")]
        public async Task<IActionResult> ShowThumbnailByFileName([FromQuery] string fileName)
        {
            string userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            bool isAdmin = User.IsInRole("ADMIN");
            var photo = _photoService.GetPhotoByFileName(fileName);
            string path = _photoService.GetPathByFileNameThumbnails(fileName);

            if (!System.IO.File.Exists(path))
                return NotFound();
            else if (!_photoService.HasAccess(photo.Id, userId, isAdmin))
                return Forbid();
            else
            {
                byte[] b = System.IO.File.ReadAllBytes(path);
                return File(b, "image/jpg");
            }
        }
        [Authorize(Roles = UserRoles.User + "," + UserRoles.Admin)]
        [HttpPut("change-photo-access-level")]
        public IActionResult ChangeAccess(int photoId)
        {
            string userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            bool isAdmin = User.IsInRole("ADMIN");

            if (!_photoService.PhotoExists(photoId) == false)
                return NotFound();
            else if (_photoService.HasPriveleges(photoId, userId, isAdmin))
                return Ok(_photoService.ChangeAccessById(photoId));
            else
                return Forbid();
        }
        [HttpGet("get_photo(s)-by-name")]
        public IActionResult GetPhotosByName(string photoName)         // 5. Przeszukiwanie zdjęć na podstawie praw użytkownika (admin dostęp do wszystkiego)
        {
            bool isAdmin = User.IsInRole("ADMIN");
            string userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var filteredPhotos = _photoService.GetPhotosByName(photoName);

            if(filteredPhotos.Count == 0)
                return NotFound();
            else 
            {
                if (isAdmin)
                    filteredPhotos = filteredPhotos;
                else
                    filteredPhotos = filteredPhotos.Where(p => p.Access == Data.AccessLevel.Public || p.UserId == userId).ToList();
            }
            

            return Ok(filteredPhotos);
        }

        [HttpGet("get-photos-by-user-name/{userName}")]
        public IActionResult GetPhotosByUserName(string userName)
        {
            bool isAdmin = User.IsInRole("ADMIN");
            string userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var filteredPhotos = _photoService.GetPhotosByAuthorName(userName);
            
            if (filteredPhotos.Count == 0)
                return NotFound();
            else
            {
                if (isAdmin)
                    filteredPhotos = filteredPhotos;
                else
                    filteredPhotos = filteredPhotos.Where(p => p.Access == Data.AccessLevel.Public || p.UserId == userId).ToList();

                return Ok(filteredPhotos);
            }
        }
        [HttpGet("get-photos-by-user-id/{authorId}")]
        public IActionResult GetPhotosByUserId(string authorId)
        {
            bool isAdmin = User.IsInRole("ADMIN");
            string userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var filteredPhotos = _photoService.GetPhotosByAuthorId(authorId);

            if (filteredPhotos.Count == 0)
                return NotFound();
            else
            {
                if (isAdmin)
                    filteredPhotos = filteredPhotos;
                else
                    filteredPhotos = filteredPhotos.Where(p => p.Access == Data.AccessLevel.Public || p.UserId == userId).ToList();

                return Ok(filteredPhotos);
            }
        }


        // 7. Zdjęcia można oceniać i dodać komentarz (oprócz własnych)


    }
}
