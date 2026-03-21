using Microsoft.EntityFrameworkCore;
using NexusReader.Shared.Models;

namespace NexusReader.Server.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<BookModel> Books { get; set; }
        public DbSet<ChapterModel> Chapters { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // This tells SQL that if a Book is deleted, delete its chapters too
            modelBuilder.Entity<BookModel>()
                .HasMany(b => b.Chapters)
                .WithOne()
                .HasForeignKey(c => c.BookModelId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}