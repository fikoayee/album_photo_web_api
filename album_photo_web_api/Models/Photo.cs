using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;
using album_photo_web_api.Data;

namespace album_photo_web_api.Models
{
    public class Photo
    {
        [Key]
        public int Id { get; set; }
        public string Name { get; set; }
        
        public string Tags { get; set; }
        public string Camera { get; set; }
        public AccessLevel Access { get; set; }
        // relationship photos >--< albums 
        public List<AlbumPhoto>? AlbumsPhotos { get; set; }
        
        // relationship photo -----< comments
        public List<Comment>? Comments { get; set; }

        // relationship user <--< photos
        public int? UserId { get; set; }
        public User User { get; set; }
    }
}
