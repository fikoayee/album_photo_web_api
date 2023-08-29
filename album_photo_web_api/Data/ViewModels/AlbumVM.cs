using System.ComponentModel.DataAnnotations;

namespace album_photo_web_api.Data.ViewModels
{

    public class AlbumVM
    {
        [Key]
        public int Id { get; set; }
        public string Name { get; set; }

        //    public string? ImageURL { get; set; }

        //    public IFormFile ImageFile { get; set; }


        //    // relacja zdjecia <--> album,  wiele do wielu
        //    public List<AlbumPhoto>? AlbumsPhotos { get; set; }

        //    //relationships
        //    public List<int>? PhotosId { get; set; }

        //}
    }
}

