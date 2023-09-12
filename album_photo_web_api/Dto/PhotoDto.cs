using album_photo_web_api.Data;
using album_photo_web_api.Models;
using System.ComponentModel.DataAnnotations.Schema;

namespace album_photo_web_api.Dto
{
    public class PhotoDto
    {
        public int Id { get; set; } 
        public string Name { get; set; }
        public string? Tags { get; set; }
        public string Camera { get; set; }
        public AccessLevel Access { get; set; }
        public string ImageName { get; set; }
        public int UpVotes { get; set; }
        public int DownVotes { get; set; }
        public List<Comment>? Comments { get; set; }
        public string? User { get; set; }
        public string UserId { get; set; }
    }
}
