using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NexusReader.Server.Data;
using NexusReader.Shared.Models;

namespace NexusReader.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class BooksController : ControllerBase
    {
        private readonly AppDbContext _db;

        public BooksController(AppDbContext db)
        {
            _db = db;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<BookSummaryResponse>>> GetBooks()
        {
            var list = await _db.Books
                .AsNoTracking()
                .OrderByDescending(b => b.UploadDate)
                .Select(b => new BookSummaryResponse(
                    b.Id,
                    b.Title,
                    b.Author,
                    b.Category,
                    b.CoverImageUrl,
                    b.ColorTheme,
                    b.Progress,
                    b.UploadDate,
                    b.Chapters.Count))
                .ToListAsync();

            return Ok(list);
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult<BookModel>> GetBook(int id)
        {
            var book = await _db.Books
                .AsNoTracking()
                .Include(b => b.Chapters.OrderBy(c => c.ChapterNumber))
                .FirstOrDefaultAsync(b => b.Id == id);

            if (book == null)
                return NotFound();

            return Ok(book);
        }

        [HttpGet("{id:int}/chapters")]
        public async Task<ActionResult<IEnumerable<ChapterListItemResponse>>> GetBookChapters(int id)
        {
            if (!await _db.Books.AnyAsync(b => b.Id == id))
                return NotFound();

            var chapters = await _db.Chapters
                .AsNoTracking()
                .Where(c => c.BookId == id)
                .OrderBy(c => c.ChapterNumber)
                .Select(c => new ChapterListItemResponse(c.Id, c.ChapterNumber, c.Title))
                .ToListAsync();

            return Ok(chapters);
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<ActionResult<BookModel>> CreateBook([FromBody] CreateBookRequest request)
        {
            var book = new BookModel
            {
                Title = request.Title,
                Author = request.Author,
                Description = request.Description,
                CoverImageUrl = request.CoverImageUrl,
                Category = request.Category,
                ColorTheme = request.ColorTheme,
                UploadDate = DateTime.UtcNow,
                Progress = 0
            };

            _db.Books.Add(book);
            await _db.SaveChangesAsync();

            return CreatedAtAction(nameof(GetBook), new { id = book.Id }, book);
        }

        [Authorize(Roles = "Admin")]
        [HttpPut("{id:int}")]
        public async Task<IActionResult> UpdateBook(int id, [FromBody] UpdateBookRequest request)
        {
            var book = await _db.Books.FindAsync(id);
            if (book == null)
                return NotFound();

            book.Title = request.Title;
            book.Author = request.Author;
            book.Description = request.Description;
            book.CoverImageUrl = request.CoverImageUrl;
            book.Category = request.Category;
            book.ColorTheme = request.ColorTheme;
            book.Progress = request.Progress;

            await _db.SaveChangesAsync();
            return NoContent();
        }

        [Authorize(Roles = "Admin")]
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> DeleteBook(int id)
        {
            var book = await _db.Books.FindAsync(id);
            if (book == null)
                return NotFound();

            _db.Books.Remove(book);
            await _db.SaveChangesAsync();
            return NoContent();
        }
    }
}
