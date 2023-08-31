using album_photo_web_api.Models;

namespace album_photo_web_api.Data.Interfaces
{
    public interface IAlbumPhotoService
    {
        AlbumPhoto GetAlbumPhotoByIds(int albumId, int photoId);
    }
}
