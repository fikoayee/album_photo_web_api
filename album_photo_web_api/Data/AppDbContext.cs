using album_photo_web_api.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Reflection.Emit;
using System.Xml.Linq;

namespace album_photo_web_api.Data
{
    public class AppDbContext :IdentityDbContext<User>
    {
        public DbSet<Photo> Photos { get; set; }
        public DbSet<Album> Albums { get; set; }
        public DbSet<AlbumPhoto> AlbumsPhotos { get; set; }
        public DbSet<Comment> Comments { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<Rate> Rates { get; set; }
        public DbSet<RefreshToken> RefreshTokens { get; set; }

        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            //AlbumPhoto
            modelBuilder.Entity<AlbumPhoto>().HasKey(ap => new
            {
                ap.Id,
            });
            modelBuilder.Entity<AlbumPhoto>().HasOne(a => a.Album).WithMany(ap => ap.AlbumsPhotos).HasForeignKey(a => a.AlbumId);
            modelBuilder.Entity<AlbumPhoto>().HasOne(a => a.Photo).WithMany(ap => ap.AlbumsPhotos).HasForeignKey(a => a.PhotoId);

            // cascade delete comments/photo
            modelBuilder.Entity<Photo>()
         .HasMany<Comment>(c => c.Comments)
         .WithOne(s => s.Photo)
         .OnDelete(DeleteBehavior.Cascade);

            base.OnModelCreating(modelBuilder);
        }
    }
}
