using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace album_photo_web_api.Models
{
    public class Rate
    {
        [Key]
        public int Id { get; set; }

        public bool IsRated { get; set; } = false;
        public string AuthorId { get; set; }
        [ForeignKey("PhotoId")]
        public Photo? Photo { get; set; }

    }
}
