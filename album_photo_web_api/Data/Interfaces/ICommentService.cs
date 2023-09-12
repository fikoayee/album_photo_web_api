using album_photo_web_api.Data.ViewModels;
using album_photo_web_api.Dto;
using album_photo_web_api.Models;

namespace album_photo_web_api.Data.Interfaces
{
    public interface ICommentService
    {
        public void AddComment(CommentVM comment, string userId, int photoId);
        public bool IsAuthor(int photoId, string userId, bool isAdmin);
        public List<CommentDto> GetAllCommentsByPhoto(int photoId);
        public bool HasPriveleges(int commentId, string userId, bool isAdmin);
        public CommentDto GetCommentById(int commentId);
        public Comment GetCommentByIdPriv(int commentId);
        public bool CommentExists(int commentId);
        public void DeleteCommentById(int commentId);

    }
}
