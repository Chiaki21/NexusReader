using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace NexusReader.Shared.Models
{
    [Table("Chapters")]
    public class ChapterModel
    {
        [Key]
        public int Id { get; set; }

        public int BookId { get; set; }
        public BookModel? Book { get; set; }

        public int ChapterNumber { get; set; }
        public string Title { get; set; } = "";
        public string Content { get; set; } = "";
    }
}
