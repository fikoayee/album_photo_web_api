using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace album_photo_web_api.Models
{
    public class Comment
    {
        [Key]
        public int Id { get; set; }
        public string Comments { get; set; }

        
        // relationship photo <--> comment
        public int? PhotoId { get; set; }
        [ForeignKey("PhotoId")]
        public Photo? Photo { get; set; }

        // relationship user <--< comments
        public int? UserId { get;set; }
        public User User { get; set; }

    }
}
