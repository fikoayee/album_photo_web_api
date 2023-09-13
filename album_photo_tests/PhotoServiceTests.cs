using album_photo_web_api.Data;
using album_photo_web_api.Data.Services;
using album_photo_web_api.Data.ViewModels;
using album_photo_web_api.Dto;
using album_photo_web_api.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace album_photo_tests
{
    public class PhotoServiceTests
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
        [Test, Order(3)]
        public void CheckIfPhotoExists_WithExistingId()
        {
            var result = photoService.PhotoExists(5);
            Assert.That(result, Is.True);
        }
        [Test, Order (4)]
        public void CheckIfPhotoExists_WithNoExistingId()
        {
            var result = photoService.PhotoExists(55);
            Assert.That(result, Is.False);
        }

        [Test, Order (5)]
        public void DeletePhotoByExistingId()
        {
            var result = photoService.PhotoExists(5);
            Assert.That(result, Is.True);
            photoService.DeletePhotoById(5);
            result = photoService.PhotoExists(5);
            Assert.That(result, Is.False);
        }

        [Test, Order(6)]
        public void ChangeAccessByExistingId()
        {
            var result = photoService.GetPhotoByIdPriv(6);
            Assert.That(result.Access, Is.EqualTo(AccessLevel.Private));
            photoService.ChangeAccessById(6);
            Assert.That(result.Access, Is.EqualTo(AccessLevel.Public));
        }
        [Test, Order(7)]
        public void GetUserIdByPhotoId()
        {
            var result = photoService.GetUserIdByPhotoId(6);
            Assert.That(result, Is.EqualTo("testowy"));
        }

        [Test, Order(7)]
        public void Votes()
        {
            photoService.AddUpvote(6);
            photoService.AddDownvote(6);
            var result = photoService.GetPhotoByIdPriv(6);
            Assert.That(result.UpVotes, Is.EqualTo(1));
            Assert.That(result.DownVotes, Is.EqualTo(1));
        }
        [Test, Order(8)]
        public void GetPhotoBy_Existing_FileName()
        {
            var result = photoService.GetPhotoByFileName("zdj2.jpg");
            Assert.That(result.ImageName, Is.EqualTo("zdj2.jpg"));
        }
        [Test, Order(9)]
        public void GetPhotoBy_Not_Existing_FileName()
        {
            Photo? result = photoService.GetPhotoByFileName("niema.jpg");
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