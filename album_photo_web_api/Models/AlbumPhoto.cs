using System.ComponentModel.DataAnnotations;

namespace album_photo_web_api.Models
{
    public class AlbumPhoto
    {
        [Key]
        public int Id { get; set; }
        public int PhotoId { get; set; }
        public Photo? Photo { get; set; }
        public int AlbumId { get; set; }
        public Album? Album { get; set; }
    }
}
