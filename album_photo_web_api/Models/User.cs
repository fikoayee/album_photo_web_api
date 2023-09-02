using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace album_photo_web_api.Models
{
    public class User : IdentityUser
    {
        // relationships
        public List<Photo>? Photos { get; set; }
        public List<Comment>? Comments { get; set; }
    }
}
