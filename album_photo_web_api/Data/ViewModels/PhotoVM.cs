using System.ComponentModel.DataAnnotations;

namespace album_photo_web_api.Data.ViewModels
{
    public class PhotoVM
    {
        public string Name { get; set; }
        public List<string> Tags { get; set; }
        public string Camera { get; set; }
        public AccessLevel Access { get; set; }
        public IFormFile ImageFile { get; set; }
    }

    public class PhotoUpdateVM
    {
        public string Name { get; set; }
        public List<string> Tags { get; set; }
        public string Camera { get; set; }
        public AccessLevel Access { get; set; }
    }
}
