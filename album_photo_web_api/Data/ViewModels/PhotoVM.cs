using album_photo_web_api.Models;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace album_photo_web_api.Data.ViewModels
{
    public class PhotoVM
    {
        public string Name { get; set; }
        public string Tags { get; set; }
        public string Camera { get; set; }
        public AccessLevel Access { get; set; }
        public IFormFile ImageFile { get; set; }
    }

    public class PhotoUpdateVM
    {
        public string Name { get; set; }
        public string Tags { get; set; }
        public string Camera { get; set; }
        public AccessLevel Access { get; set; }
    }

}
