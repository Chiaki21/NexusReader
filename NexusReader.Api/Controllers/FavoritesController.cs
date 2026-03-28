using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NexusReader.Server.Data;
using NexusReader.Shared.Models;

namespace NexusReader.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class FavoritesController : ControllerBase
    {
        private readonly AppDbContext _db;

        public FavoritesController(AppDbContext db)
        {
            _db = db;
        }

        private string? UserId => User.FindFirstValue(ClaimTypes.NameIdentifier);

        [HttpGet]
        public async Task<ActionResult<IEnumerable<BookSummaryResponse>>> GetFavorites()
        {
            var uid = UserId;
            if (string.IsNullOrEmpty(uid))
                return Unauthorized();

            var books = await _db.Favorites
                .AsNoTracking()
                .Where(f => f.UserId == uid)
                .OrderByDescending(f => f.Id)
                .Select(f => new BookSummaryResponse(
                    f.Book!.Id,
                    f.Book.Title,
                    f.Book.Author,
                    f.Book.Category,
                    f.Book.CoverImageUrl,
                    f.Book.ColorTheme,
                    f.Book.Progress,
                    f.Book.UploadDate,
                    f.Book.Chapters.Count))
                .ToListAsync();

            return Ok(books);
        }

        [HttpPost]
        public async Task<IActionResult> AddFavorite([FromBody] FavoriteRequest request)
        {
            var uid = UserId;
            if (string.IsNullOrEmpty(uid))
                return Unauthorized();

            if (!await _db.Books.AnyAsync(b => b.Id == request.BookId))
                return NotFound();

            if (await _db.Favorites.AnyAsync(f => f.UserId == uid && f.BookId == request.BookId))
                return Ok();

            _db.Favorites.Add(new FavoriteModel { UserId = uid, BookId = request.BookId });
            await _db.SaveChangesAsync();
            return Ok();
        }

        [HttpDelete("{bookId:int}")]
        public async Task<IActionResult> RemoveFavorite(int bookId)
        {
            var uid = UserId;
            if (string.IsNullOrEmpty(uid))
                return Unauthorized();

            var fav = await _db.Favorites.FirstOrDefaultAsync(f => f.UserId == uid && f.BookId == bookId);
            if (fav == null)
                return NotFound();

            _db.Favorites.Remove(fav);
            await _db.SaveChangesAsync();
            return NoContent();
        }
    }
}
