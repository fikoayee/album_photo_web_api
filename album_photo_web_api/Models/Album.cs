﻿using album_photo_web_api.Data;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;

namespace album_photo_web_api.Models
{
    public class Album
    {
        [Key]
        public int Id { get; set; }
        public string Name { get; set; }

        // relationship photos >--< albums
        public List<AlbumPhoto>? AlbumsPhotos { get; set; }
    }
}