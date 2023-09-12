using album_photo_web_api.Data;
using album_photo_web_api.Data.Services;
using album_photo_web_api.Dto;
using album_photo_web_api.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;

namespace album_photo_tests
{
    public class PhotoServiceTest
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
            photoService = new PhotoService(context, webHostEnvironment);
            albumService = new AlbumService(context);
            SeedDatabase();
        }

        [Test, Order(1)]
        public void GetPhotoById()
        {
            var result = photoService.GetPhotoByIdPriv(5);
            Assert.That(result.Id, Is.EqualTo(5));
            Assert.That(result.Name, Is.EqualTo("Piesek publiczne"));
        }

        [Test, Order(2)]
        public void GetPhotoById_WithWrongId()
        {
            var result = photoService.GetPhotoByIdPriv(94);
            Assert.That(result, Is.Null);
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
                    PhotoId = 1,
                },
                new AlbumPhoto()
                {
                    AlbumId = 1,
                    PhotoId = 2,
                },
                new AlbumPhoto()
                {
                    AlbumId = 2,
                    PhotoId = 1,
                }
            };
            context.AlbumsPhotos.AddRange(ap);
            context.SaveChanges();
        }
    }
}