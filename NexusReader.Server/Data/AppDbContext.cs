using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using NexusReader.Shared.Models;
using Microsoft.AspNetCore.Identity;

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

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // IMPORTANT: Call the base method to set up Identity tables!
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