using System.ComponentModel.DataAnnotations;

namespace album_photo_web_api.Models
{
    public class User
    {
        [Key]
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }

        // relationships
        public List<Photo>? Photos { get; set; }
        public List<Comment>? Comments { get; set; }
    }
}
