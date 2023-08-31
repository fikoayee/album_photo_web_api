using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using album_photo_web_api.Data.Services;
using album_photo_web_api.Data.ViewModels;
using album_photo_web_api.Models;

namespace album_photo_web_api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AlbumController : ControllerBase
    {
        public AlbumService _albumService;
        public AlbumController(AlbumService albumService)
        {
            _albumService = albumService;
        }

        [HttpGet("get-all-albums")]
        public IActionResult GetAlbums()
        {
            var allAlbums = _albumService.GetAllAlbums();
            return Ok(allAlbums);
        }

        [HttpGet("get-album-by-id/{albumId}")]
        public IActionResult GetAlbumById(int albumId)
        {
            var album = _albumService.GetAlbumById(albumId);
            return Ok(album);
        }

        [HttpPut("update-album-by-id/{albumId}")]
        public IActionResult UpdateAlbumById(int albumId, [FromForm] AlbumVM album)
        {
            var albumUpd = _albumService.UpdateAlbumById(albumId, album);
            return Ok(albumUpd);
        }
        [HttpDelete("delete-album-by-id/{albumId}")]
        public IActionResult DeleteAlbumById(int albumId)
        {
            _albumService.DeleteAlbumById(albumId);
            return Ok();
        }
        [HttpPost("add-album")]
        public IActionResult AddAlbumWithPhoto([FromForm] AlbumVM album)
        {
            _albumService.UploadPhoto(album.ImageFile);
            _albumService.AddAlbum(album);
            return Ok();
        }
        [HttpPut("move-photos-between-albums")]
        public IActionResult MovePhotosBetweenAlbums(int currentAlbumId, int destinationAlbumId, List<int> PhotoIds)
        {
            _albumService.MovePhotos(currentAlbumId, destinationAlbumId, PhotoIds);
            return Ok();
        }
        [HttpDelete("remove-photos-by-ids")]
        public IActionResult RemovePhotosByIds(int albumId, List<int> photoIds)
        {
            _albumService.RemovePhotoByIds(albumId, photoIds);
            return Ok();
        }
        [HttpPost("add-photos-by-ids")]
        public IActionResult AddPhotosByIds(int albumId, List<int> photoIds)
        {
            _albumService.AddPhotoByIds(albumId, photoIds);
            return Ok();
        }
    }
}
