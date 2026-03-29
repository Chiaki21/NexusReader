using System.ComponentModel.DataAnnotations;

namespace NexusReader.Shared.Models
{
    public class BookUploadModel
    {
        [Required(ErrorMessage = "Title is required")]
        public string Title { get; set; } = string.Empty;

        [Required(ErrorMessage = "Author name is required")]
        public string Author { get; set; } = string.Empty;

        public string Description { get; set; } = string.Empty;

        [Required(ErrorMessage = "Please select a genre")]
        public string Genre { get; set; } = string.Empty;

        [Required(ErrorMessage = "Book content is required")]
        public string Content { get; set; } = string.Empty;

        // We store the image as a Base64 string for easy transfer via JSON
        public string? CoverImageBase64 { get; set; }
        
        public string? ImageContentType { get; set; }
        
        public bool IsPublic { get; set; } = true;
    }
}