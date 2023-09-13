using album_photo_web_api.Data;
using album_photo_web_api.Data.Interfaces;
using album_photo_web_api.Data.Services;
using album_photo_web_api.Data.ViewModels;
using album_photo_web_api.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Data;
using System.Security.Claims;

namespace album_photo_web_api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CommentController : ControllerBase
    {
        public CommentService _commentService;
        public PhotoService _photoService;
        public CommentController(CommentService commentService, PhotoService photoService)
        {
            _commentService = commentService;
            _photoService = photoService;
        }
        [HttpGet("get-all-comments-by-photo/{photoId}")]
        public IActionResult GetCommentsByPhotoId(int photoId)
        {
            string userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            bool isAdmin = User.IsInRole("ADMIN");
            if (_photoService.PhotoExists(photoId) == false || _photoService.HasAccess(photoId, userId, isAdmin) == false)
                return NotFound();
            else
            {
                var obj = _commentService.GetAllCommentsByPhoto(photoId);
                return Ok(obj);
            }



        }

        [Authorize(Roles = UserRoles.User + "," + UserRoles.Admin)]
        [HttpPost("add-comment-to-photo/{photoId}")]
        public IActionResult AddCommentToPhoto(int photoId, CommentVM comment)
        {
            var photo = _photoService.GetPhotoById(photoId);
            string userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            bool isAdmin = User.IsInRole("ADMIN");

            if (photo == null || string.IsNullOrWhiteSpace(comment.Comment) == true)
                return BadRequest();
            else
            {
                bool hasAccess = _photoService.HasAccess(photoId, userId, isAdmin);
                bool isAuthor = _commentService.IsAuthor(photoId, userId, isAdmin);
                if (isAuthor || hasAccess == false)
                    return Forbid();
                else
                {
                    _commentService.AddComment(comment, userId, photoId);
                    return Ok();
                }
            }
        }

        [Authorize(Roles = UserRoles.User + "," + UserRoles.Admin)]
        [HttpGet("delete-comment-by-id/{commentId}")]
        public IActionResult DeleteCommentById(int commentId)
        {
            string userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            bool isAdmin = User.IsInRole("ADMIN");

            if (_commentService.CommentExists(commentId) == false)
                return NotFound();
            else if (_commentService.HasPriveleges(commentId, userId, isAdmin) == false)
                return Forbid();
            else
            {
                _commentService.DeleteCommentById(commentId);
                return Ok();
            }
        }
    }
}
