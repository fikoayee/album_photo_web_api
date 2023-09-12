using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using album_photo_web_api.Data.Services;
using album_photo_web_api.Data.ViewModels;
using album_photo_web_api.Models;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using album_photo_web_api.Data.Interfaces;
using album_photo_web_api.Data;

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
            bool isAdmin = User.IsInRole("ADMIN");
            string userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (isAdmin)
                allAlbums = allAlbums;
            else
                allAlbums = allAlbums.Where(p => p.Access == Data.AccessLevel.Public || p.UserId == userId).ToList();

            return Ok(allAlbums);
        }

        [HttpGet("get-album-by-id/{albumId}")]
        public IActionResult GetAlbumById(int albumId)
        {
            var album = _albumService.GetAlbumById(albumId);
            if (album == null)
                return NotFound();
            else
            {
                string userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                bool isAdmin = User.IsInRole("ADMIN");

                if (!_albumService.HasAccess(albumId, userId, isAdmin))
                    return Forbid();
                return Ok(album);
            }
        }

        [Authorize(Roles = UserRoles.User + "," + UserRoles.Admin)]
        [HttpPut("update-album-by-id/{albumId}")]
        public IActionResult UpdateAlbumById(int albumId, [FromForm] AlbumUpdateVM album)
        {
            string userId = _albumService.GetUserIdByAlbumId(albumId);
            bool isAdmin = User.IsInRole("ADMIN");
            string currUserId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (!_albumService.AlbumExists(albumId))
            {
                return NotFound();
            }
            else
            {
                if (_albumService.HasPriveleges(albumId, currUserId, isAdmin))
                {
                    var photoUpd = _albumService.UpdateAlbumById(albumId, album, userId);
                    return Ok(photoUpd); ;
                }
                else
                    return Forbid();
            }
        }
        
        [Authorize(Roles = UserRoles.User + "," + UserRoles.Admin)]
        [HttpDelete("delete-album-by-id/{albumId}")]
        public IActionResult DeleteAlbumById(int albumId)
        {
            bool isAdmin = User.IsInRole("ADMIN");
            string currUserId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (!_albumService.AlbumExists(albumId))
            {
                return NotFound();
            }
            else
            {
                if (_albumService.HasPriveleges(albumId, currUserId, isAdmin))
                {
                    _albumService.DeleteAlbumById(albumId);
                    return Ok();
                }
                else
                    return Forbid();
            }
        }
        [Authorize(Roles = UserRoles.User + "," + UserRoles.Admin)]
        [HttpPost("add-album")]
        public IActionResult AddAlbumWithPhoto([FromForm] AlbumVM album)
        {
            string userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            bool isAdmin = User.IsInRole("ADMIN");


            _albumService.AddAlbum(album, userId, isAdmin);
            return Ok(album);
        }
        
        [Authorize(Roles = UserRoles.User + "," + UserRoles.Admin)]
        [HttpPut("move-photos-between-albums")]
        public IActionResult MovePhotosBetweenAlbums(int currentAlbumId, int destinationAlbumId, List<int> PhotoIds)
        {
            string currUserId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            bool isAdmin = User.IsInRole("ADMIN");
            
            bool statusChanged = _albumService.MovePhotos(isAdmin, currUserId, currentAlbumId, destinationAlbumId, PhotoIds);
            if (statusChanged)
                return Ok();
            else
                return BadRequest();
        }
        
        [Authorize(Roles = UserRoles.User + "," + UserRoles.Admin)]
        [HttpDelete("remove-photos-by-ids")]
        public IActionResult RemovePhotosByIds(int albumId, List<int> photoIds)
        {
            string currUserId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            bool isAdmin = User.IsInRole("ADMIN");

            bool statusChanged = _albumService.RemovePhotoByIds(isAdmin, currUserId, albumId, photoIds);
            if (statusChanged)
                return Ok();
            else
                return BadRequest();
        }

        [Authorize(Roles = UserRoles.User + "," + UserRoles.Admin)]
        [HttpPost("add-photos-by-ids")]
        public IActionResult AddPhotosByIds(int albumId, List<int> photoIds)
        {
            string currUserId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            bool isAdmin = User.IsInRole("ADMIN");

            bool statusChanged = _albumService.AddPhotoByIds(isAdmin, currUserId, albumId, photoIds);
            if (statusChanged)
                return Ok();
            else
                return BadRequest();
        }
        [HttpGet("get-album(s)-by-name/{albumName}")]
        public IActionResult GetAlbumsByName(string albumName)
        {
            bool isAdmin = User.IsInRole("ADMIN");
            string userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var filteredAlbums = _albumService.GetAlbumsByName(albumName);

            if (filteredAlbums.Count == 0)
                return NotFound();
            else
            {
                if (isAdmin)
                    filteredAlbums = filteredAlbums;
                else
                    filteredAlbums = filteredAlbums.Where(p => p.Access == Data.AccessLevel.Public || p.UserId == userId).ToList();
            }

            return Ok(filteredAlbums);
        }
        [HttpGet("get-albums-by-user-name/{userName}")]
        public IActionResult GetAlbumsByUserName(string userName)
        {
            bool isAdmin = User.IsInRole("ADMIN");
            string userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var filteredAlbums = _albumService.GetAlbumsByAuthorName(userName);

            if (filteredAlbums == null)
                return NotFound();
            else if (filteredAlbums.Count == 0)
                return NotFound();
            else
            {
                if (isAdmin)
                    filteredAlbums = filteredAlbums;
                else
                    filteredAlbums = filteredAlbums.Where(p => p.Access == Data.AccessLevel.Public || p.UserId == userId).ToList();

                return Ok(filteredAlbums);
            }
        }
        [HttpGet("get-albums-by-user-id/{authorId}")]
        public IActionResult GetAlbumsByUserId(string authorId)
        {
            bool isAdmin = User.IsInRole("ADMIN");
            string userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var filteredAlbums = _albumService.GetAlbumsByAuthorId(authorId);

            if (filteredAlbums == null)
                return NotFound();
            else if (filteredAlbums.Count == 0)
                return NotFound();
            else
            {
                if (isAdmin)
                    filteredAlbums = filteredAlbums;
                else
                    filteredAlbums = filteredAlbums.Where(p => p.Access == Data.AccessLevel.Public || p.UserId == userId).ToList();

                return Ok(filteredAlbums);
            }
        }

        [Authorize(Roles = UserRoles.User + "," + UserRoles.Admin)]
        [HttpPut("change-album-access-level/{albumId}")]
        public IActionResult ChangeAccess(int albumId)
        {
            string userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            bool isAdmin = User.IsInRole("ADMIN");

            if (_albumService.AlbumExists(albumId) == false)
                return NotFound();
            else if (_albumService.HasPriveleges(albumId, userId, isAdmin))
                return Ok(_albumService.ChangeAccessById(albumId));
            else
                return Forbid();
        }
        [HttpPut("change-album-and-photo-access-level")]
        public IActionResult ChangeAccessForAll(int albumId)
        {
            string userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            bool isAdmin = User.IsInRole("ADMIN");

            if (_albumService.AlbumExists(albumId) == false)
                return NotFound();
            else if (_albumService.HasPriveleges(albumId, userId, isAdmin))
                return Ok(_albumService.ChangeAccessByIdForAll(albumId));
            else
                return Forbid();
        }

        [HttpPut("delete-album-with-photos")]
        public IActionResult DeleteAlbumWithPhotos(int albumId)
        {
            string userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            bool isAdmin = User.IsInRole("ADMIN");

            if (_albumService.AlbumExists(albumId) == false)
                return NotFound();
            else if (_albumService.HasPriveleges(albumId, userId, isAdmin))
                return Ok(_albumService.DeleteAlbumWithPhotos(albumId));
            else
                return Forbid();
        }
    }
}
