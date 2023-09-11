using album_photo_web_api.Data;
using album_photo_web_api.Models;

namespace album_photo_web_api.Dto
{
    public class AlbumDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public AccessLevel Access { get; set; }
        public string? User { get; set; }
        public string UserId { get; set; }
        public List<PhotoDto> Photos { get; set; }
        
    }
}
