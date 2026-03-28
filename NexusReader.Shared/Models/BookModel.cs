using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace NexusReader.Shared.Models
{
    [Table("Books")]
    public class BookModel
    {
        [Key]
        public int Id { get; set; }

        public string Title { get; set; } = "";
        public string Author { get; set; } = "";
        public string Description { get; set; } = "";
        public string? CoverImageUrl { get; set; }
        public string Category { get; set; } = "Romance";
        public DateTime UploadDate { get; set; } = DateTime.UtcNow;

        public string ColorTheme { get; set; } = "blush";
        public int Progress { get; set; }

        public List<ChapterModel> Chapters { get; set; } = new();
    }
}
