using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using NexusReader.Shared.Models;

namespace NexusReader.Server.Data
{
    // Inheriting from IdentityDbContext handles the AspNetUsers and AspNetRoles tables
    public class AppDbContext : IdentityDbContext<IdentityUser>
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<BookModel> Books { get; set; }
        public DbSet<ChapterModel> Chapters { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Crucial: Call the base method to ensure Identity tables are configured
            base.OnModelCreating(modelBuilder);

            // This tells SQL that if a Book is deleted, delete its chapters too
            modelBuilder.Entity<BookModel>()
                .HasMany(b => b.Chapters)
                .WithOne()
                .HasForeignKey(c => c.BookModelId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}