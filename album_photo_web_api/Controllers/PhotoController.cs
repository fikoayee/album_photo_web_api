using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using album_photo_web_api.Data.Services;
using album_photo_web_api.Data.ViewModels;
using album_photo_web_api.Models;

namespace album_photo_web_api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PhotoController : ControllerBase
    {
        public PhotoService _photoService;
        public PhotoController(PhotoService photoService)
        {
            _photoService = photoService;
        }

        [HttpPost("add-photo")]
        public IActionResult AddPhoto([FromBody] PhotoVM photo)
        {
            _photoService.AddPhoto(photo);
            return Ok();
        }

        [HttpGet("get-all-photos")]
        public IActionResult GetPhotos()
        {
            var allPhotos = _photoService.GetAllPhotos();
            return Ok(allPhotos);
        }

        [HttpGet("get-photo-by-id/{photoId}")]
        public IActionResult GetPhotoById(int albumId)
        {
            var photo = _photoService.GetPhotoById(albumId);
            return Ok(photo);
        }

        [HttpPut("update-photo-by-id/{photoId}")]
        public IActionResult UpdatePhotoById(int photoId, [FromBody] PhotoVM photo)
        {
            var photoUpd = _photoService.UpdatePhotoById(photoId, photo);
            return Ok(photoUpd);
        }
        [HttpDelete("delete-photo-by-id/{photoId}")]
        public IActionResult DeletePhotoById(int photoId)
        {
            _photoService.DeletePhotoById(photoId);
            return Ok();
        }
    }
}
