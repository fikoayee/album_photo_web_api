using album_photo_web_api.Data.Services;
using album_photo_web_api.Data;
using album_photo_web_api.Dto;
using album_photo_web_api.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace album_photo_tests
{
    public class AlbumServiceTests
    {
        private static DbContextOptions<AppDbContext> dbContextOptions = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(databaseName: "AppDbTest")
            .Options;

        AppDbContext context;
        PhotoService photoService;
        AlbumService albumService;
        IWebHostEnvironment webHostEnvironment;

        [OneTimeSetUp] // setup db only one time
        public void Setup()
        {
            context = new AppDbContext(dbContextOptions);
            context.Database.EnsureCreated();

            SeedDatabase();

            photoService = new PhotoService(context, webHostEnvironment);
            albumService = new AlbumService(context);
            var albumy = context.Albums;
            var zdjecia = context.Photos;
        }

        [Test, Order(1)]
        public void GetAlbumById()
        {
            var result = albumService.GetAlbumByIdPriv(1);
            Assert.That(result.Id, Is.EqualTo(1));
            Assert.AreEqual(typeof(Album), result.GetType());
        }
        [Test, Order(2)]
        public void GetAlbumById_WithWrongId()
        {
            var result = albumService.GetAlbumByIdPriv(55);
            Assert.That(result, Is.Null);
        }
        [Test, Order(3)]
        public void CheckIfAlbumExists_WithExistingId()
        {
            var result = albumService.AlbumExists(1);
            Assert.That(result, Is.True);
        }
        [Test, Order(4)]
        public void CheckIfAlbumExists_WithNoExistingId()
        {
            var result = albumService.AlbumExists(55);
            Assert.That(result, Is.False);
        }
        [Test, Order(5)]
        public void ChangeAccessBy_ExistingId()
        {
            var result = albumService.GetAlbumByIdPriv(1);
            Assert.That(result.Access, Is.EqualTo(AccessLevel.Public));
            albumService.ChangeAccessById(1);
            Assert.That(result.Access, Is.EqualTo(AccessLevel.Private));
        }

        [Test, Order(6)]
        public void GetUserIdByPhotoAndAlbumIds_NonExistingIds()
        {
            var result = albumService.ChangeAccessById(55);
            Assert.That(result, Is.Null);
        }

        [Test, Order(7)]
        public void GetAlbumPhotoByIds()
        {
            var result = albumService.GetAlbumPhotoByIds(2, 6);
            Assert.That(result.Id, Is.EqualTo(3));
        }

        [Test, Order(8)]
        public void ChangeAccessByIdForAll()
        {
            var result = albumService.ChangeAccessByIdForAll(2);
            var album = albumService.GetAlbumByIdPriv(2);
            var photo5 = photoService.GetPhotoByIdPriv(5);
            var photo6 = photoService.GetPhotoByIdPriv(6);
            Assert.That(album.Access, Is.EqualTo(AccessLevel.Private));
            Assert.That(photo5.Access, Is.EqualTo(AccessLevel.Private));
            Assert.That(photo6.Access, Is.EqualTo(AccessLevel.Private));
        }

        [Test, Order(9)]
        public void DeleteAlbumWithPhotos()
        {
            albumService.DeleteAlbumWithPhotos(2);

            var photo1 = photoService.PhotoExists(5);
            var photo2 = photoService.PhotoExists(6);
            var album = albumService.AlbumExists(2);

            Assert.That(photo1, Is.EqualTo(false));
            Assert.That(photo2, Is.EqualTo(false));
            Assert.That(album, Is.EqualTo(false));
        }

        [Test, Order(10)]
        public void GetUserIdByAlbumId()
        {
            var result = albumService.GetUserIdByAlbumId(1);
            Assert.That(result, Is.EqualTo("testowy"));
        }

        [OneTimeTearDown]
        public void CleanUp()
        {
            context.Database.EnsureDeleted();
        }

        private void SeedDatabase()
        {
            var photos = new List<Photo>()
            {
                new Photo()
                {
                    Id = 5,
                    Access = AccessLevel.Public,
                    Camera = "Nikon 205",
                    ImageName = "zdj1.jpg",
                    Name = "Piesek publiczne",
                    Tags = "tag",
                    UserId = "testowy",
                },
                new Photo()
                {
                    Id = 6,
                    Access = AccessLevel.Private,
                    Camera = "Sony LEM",
                    ImageName = "zdj2.jpg",
                    Name = "Kotek prywatne",
                    Tags = "tag",
                    UserId = "testowy",
                },
            };
            context.Photos.AddRange(photos);

            var photosDto = new List<PhotoDto>()
            {
                new PhotoDto()
                {
                    Id = 5,
                    Access = AccessLevel.Public,
                    Camera = "Nikon 205",
                    ImageName = "zdj1.jpg",
                    Name = "Piesek publiczne",
                    Tags = "tag",
                    UserId = "testowy",
                },
                new PhotoDto()
                {
                    Id = 6,
                    Access = AccessLevel.Private,
                    Camera = "Sony LEM",
                    ImageName = "zdj2.jpg",
                    Name = "Kotek prywatne",
                    Tags = "tag",
                    UserId = "testowy",
                },

            };

            var albums = new List<Album>()
            {
                new Album()
                {
                    Id = 1,
                    Access = AccessLevel.Public,
                    Name = "Zwierzaki",
                    UserId = "testowy"
                },
                new Album()
                {
                    Id = 2,
                    Access = AccessLevel.Public,
                    Name = "Tylko Piesek",
                    UserId = "Testowy"
                },
            };
            context.Albums.AddRange(albums);

            var ap = new List<AlbumPhoto>()
            {
                new AlbumPhoto()
                {
                    AlbumId = 1,
                    PhotoId = 6,
                },
                new AlbumPhoto()
                {
                    AlbumId = 2,
                    PhotoId = 5,
                },
                new AlbumPhoto()
                {
                    AlbumId = 2,
                    PhotoId = 6,
                }
            };
            context.AlbumsPhotos.AddRange(ap);

            var comments = new List<Comment>()
            {
                new Comment()
                {
                    Comments = "pierwszy komentarz dla pieska",
                    Id = 1,
                    PhotoId= 1,
                    UserId = "testowy",
                }
            };
            context.Comments.AddRange(comments);



            context.SaveChanges();
        }
    }
}

