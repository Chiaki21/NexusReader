using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace NexusReader.Shared.Models
{
    [Table("Favorites")]
    public class FavoriteModel
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string UserId { get; set; } = "";

        public int BookId { get; set; }

        public ApplicationUser? User { get; set; }
        public BookModel? Book { get; set; }
    }
}
