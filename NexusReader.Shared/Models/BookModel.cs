using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace NexusReader.Shared.Models
{
    [Table("Books")] // Tells EF this class maps to the "Books" table in SQL
    public class BookModel
    {
        [Key] // Explicitly marks this as the Primary Key
        public int Id { get; set; }
        
        public string Title { get; set; } = "";
        public string Author { get; set; } = "";
        public string Category { get; set; } = "Romance";
        public string ColorTheme { get; set; } = "blush";
        public string Description { get; set; } = "";
        public string? CoverImage { get; set; }
        public int Progress { get; set; }
        
        
        public List<ChapterModel> Chapters { get; set; } = new();
    }

    [Table("Chapters")]
    public class ChapterModel
    {
        [Key]
        public int Id { get; set; }
        
        // --- ADD THIS LINE ---
        public int BookModelId { get; set; } 
        
        public string Title { get; set; } = "";
        public string Content { get; set; } = "";
    }
}