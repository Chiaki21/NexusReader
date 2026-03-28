using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using NexusReader.Shared.Models;

namespace NexusReader.Server.Data
{
    public class AppDbContext : IdentityDbContext<ApplicationUser>
    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options)
        {
        }

        public DbSet<BookModel> Books { get; set; }
        public DbSet<ChapterModel> Chapters { get; set; }
        public DbSet<FavoriteModel> Favorites { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<BookModel>()
                .HasMany(b => b.Chapters)
                .WithOne(c => c.Book)
                .HasForeignKey(c => c.BookId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<FavoriteModel>()
                .HasOne(f => f.User)
                .WithMany()
                .HasForeignKey(f => f.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<FavoriteModel>()
                .HasOne(f => f.Book)
                .WithMany()
                .HasForeignKey(f => f.BookId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<FavoriteModel>()
                .HasIndex(f => new { f.UserId, f.BookId })
                .IsUnique();
        }
    }
}
