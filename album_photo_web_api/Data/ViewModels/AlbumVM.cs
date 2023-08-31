using System.ComponentModel.DataAnnotations;

namespace album_photo_web_api.Data.ViewModels
{

    public class AlbumVM
    {
        [Key]
        public string Name { get; set; }
        public AccessLevel Access { get; set; }
        public List<int>? PhotoId { get; set; }
        public IFormFile ImageFile { get; set; }

    }

    public class AlbumWithPhotosVM
    {
        [Key]
        public int Id { get; set; }
        public string Name { get; set; }
        public List<string> PhotoNames { get; set; }
    }
}

