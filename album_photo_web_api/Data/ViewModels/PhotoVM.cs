using System.ComponentModel.DataAnnotations;

namespace album_photo_web_api.Data.ViewModels
{
    public class PhotoVM
    {
        [Key]
        public int Id { get; set; }
        public string Name { get; set; }
        public List<int>? AlbumId { get; set; }
    }
}
