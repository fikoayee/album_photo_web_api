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

        [HttpPost("add-album")]
        public IActionResult AddAlbum([FromBody] AlbumVM album)
        {
            _albumService.AddAlbum(album);
            return Ok();
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
        public IActionResult UpdateAlbumById(int albumId, [FromBody] AlbumVM album)
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
    }
}
