using album_photo_web_api.Data.Interfaces;
using album_photo_web_api.Data.ViewModels;
using album_photo_web_api.Dto;
using album_photo_web_api.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace album_photo_web_api.Data.Services
{
    public class CommentService : ICommentService
    {
        private AppDbContext _context;
        private PhotoService _photoService;
        public CommentService(AppDbContext context, PhotoService photoService)
        {
            _context = context;
            _photoService = photoService;
        }

        public async void AddComment([FromForm] CommentVM comment, string userId, int photoId)
        {
            var newComment = new Comment()
            {
                Comments = comment.Comment,
                UserId = userId,
                PhotoId = photoId,
            };
            _context.Add(newComment);
            _context.SaveChanges();
        }
        public bool IsAuthor(int photoId, string userId, bool isAdmin)
        {
            var photo = _photoService.GetPhotoByIdPriv(photoId);
            if (photo.UserId == userId && isAdmin == false)
                return true;
            else
                return false;
        }
        public List<CommentDto> GetAllCommentsByPhoto(int photoId)
        {
            var photo = _context.Photos.Include(c => c.Comments).Include(u => u.User).FirstOrDefault(p => p.Id == photoId);
            var comments = photo.Comments;
            var commentsDto = new List<CommentDto>();
            foreach (var comment in comments)
            {
                CommentDto obj = new CommentDto()
                {
                    Id = comment.Id,
                    Comment = comment.Comments,
                    AuthorId = comment.UserId
                };
                commentsDto.Add(obj);
            }
            return commentsDto;
        }
        public bool HasPriveleges(int commentId, string userId, bool isAdmin)
        {
            var comment = GetCommentById(commentId);

            if (isAdmin || userId == comment.AuthorId)
                return true;
            else
                return false;
        }
        public CommentDto GetCommentById(int commentId)
        {
            var comment = _context.Comments.Include(u => u.User).FirstOrDefault(c => c.Id == commentId);
            if (comment == null)
                return null;
            else
            {
                CommentDto obj = new CommentDto()
                {
                    Id = comment.Id,
                    Comment = comment.Comments,
                    AuthorId = comment.User.Id
                };
                return (obj);
            }
        }

        public Comment GetCommentByIdPriv(int commentId)
        {
            var comment = _context.Comments.Include(u => u.User).FirstOrDefault(c => c.Id == commentId);
            if (comment == null)
                return null;
            else
                return comment;
        }

        public bool CommentExists(int commentId)
        {
            var comment = GetCommentById(commentId);
            if (comment == null)
                return false;
            else
                return true;
        }
        public void DeleteCommentById(int commentId)
        {
            if (CommentExists(commentId))
            {
                var comment = GetCommentByIdPriv(commentId);
                _context.Comments.Remove(comment);
                _context.SaveChanges();
            }
        }
    }
}
