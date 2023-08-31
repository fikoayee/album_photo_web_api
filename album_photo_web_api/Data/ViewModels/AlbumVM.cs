using System.ComponentModel.DataAnnotations;

namespace album_photo_web_api.Data.ViewModels
{

    public class AlbumVM
    {
        public string Name { get; set; }
        public AccessLevel Access { get; set; }
        public List<int>? PhotoId { get; set; }
        public IFormFile ImageFile { get; set; }

    }

    public class AlbumWithPhotosVM
    {
        
    }
}

