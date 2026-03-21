using NexusReader.Shared.Models;

namespace NexusReader.Services
{
    public class BookService
    {
        public List<BookModel> AllBooks { get; set; } = new();

        public BookService()
        {
            // Initializing with more colors and variety
            AllBooks = new List<BookModel>
            {
                new BookModel { Id = 1, Title = "Book Lovers", Author = "Emily Henry", Category = "Romance", ColorTheme = "peach", Progress = 32, Description = "One summer. Two rivals. A plot twist they didn't see coming." },
                new BookModel { Id = 2, Title = "Pride and Prejudice", Author = "Jane Austen", Category = "Classic", ColorTheme = "slate", Progress = 100, Description = "A timeless story of manners, upbringing, and marriage." },
                new BookModel { Id = 3, Title = "Evelyn Hugo", Author = "Taylor Jenkins Reid", Category = "Contemporary", ColorTheme = "emerald", Progress = 0, Description = "The glamorous and scandalous life of a Hollywood icon." },
                new BookModel { Id = 4, Title = "Thorns and Roses", Author = "Sarah J. Maas", Category = "Fantasy", ColorTheme = "lavender", Progress = 12, Description = "A captivating tale of faerie lands and dangerous bargains." },
                new BookModel { Id = 5, Title = "The Love Hypothesis", Author = "Ali Hazelwood", Category = "Romance", ColorTheme = "blush", Progress = 0, Description = "When a fake relationship between scientists meets the irresistible force of attraction." },
                new BookModel { Id = 6, Title = "Beach Read", Author = "Emily Henry", Category = "Romance", ColorTheme = "sky", Progress = 100, Description = "Two writers, one summer, and a challenge to swap genres." },
                new BookModel { Id = 7, Title = "Dark Matter", Author = "Blake Crouch", Category = "Sci-Fi", ColorTheme = "slate", Progress = 50, Description = "A mind-bending thriller about the paths not taken." },
                new BookModel { Id = 8, Title = "Circe", Author = "Madeline Miller", Category = "Fantasy", ColorTheme = "rose", Progress = 5, Description = "In the house of Helios, a daughter is born with the power of witchcraft." }
            };
        }

        public BookModel? GetBookById(int id) => AllBooks.FirstOrDefault(b => b.Id == id);
    }
}