using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NexusReader.Server.Data;
using NexusReader.Shared.Models;

namespace NexusReader.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ChaptersController : ControllerBase
    {
        private readonly AppDbContext _db;

        public ChaptersController(AppDbContext db)
        {
            _db = db;
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult<ChapterContentResponse>> GetChapter(int id)
        {
            var chapter = await _db.Chapters
                .AsNoTracking()
                .Include(c => c.Book)
                .FirstOrDefaultAsync(c => c.Id == id);

            if (chapter?.Book == null)
                return NotFound();

            var siblings = await _db.Chapters
                .AsNoTracking()
                .Where(c => c.BookId == chapter.BookId)
                .OrderBy(c => c.ChapterNumber)
                .Select(c => new { c.Id, c.ChapterNumber })
                .ToListAsync();

            int? prevId = null;
            int? nextId = null;
            for (var i = 0; i < siblings.Count; i++)
            {
                if (siblings[i].Id != id) continue;
                if (i > 0) prevId = siblings[i - 1].Id;
                if (i < siblings.Count - 1) nextId = siblings[i + 1].Id;
                break;
            }

            return Ok(new ChapterContentResponse(
                chapter.Id,
                chapter.BookId,
                chapter.ChapterNumber,
                chapter.Title,
                chapter.Content,
                chapter.Book.Title,
                nextId,
                prevId));
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<ActionResult<ChapterModel>> CreateChapter([FromBody] UpsertChapterRequest request)
        {
            if (!await _db.Books.AnyAsync(b => b.Id == request.BookId))
                return BadRequest("Invalid book.");

            var chapter = new ChapterModel
            {
                BookId = request.BookId,
                ChapterNumber = request.ChapterNumber,
                Title = request.Title,
                Content = request.Content
            };

            _db.Chapters.Add(chapter);
            await _db.SaveChangesAsync();

            return CreatedAtAction(nameof(GetChapter), new { id = chapter.Id }, chapter);
        }

        [Authorize(Roles = "Admin")]
        [HttpPut("{id:int}")]
        public async Task<IActionResult> UpdateChapter(int id, [FromBody] UpsertChapterRequest request)
        {
            var chapter = await _db.Chapters.FindAsync(id);
            if (chapter == null)
                return NotFound();

            chapter.BookId = request.BookId;
            chapter.ChapterNumber = request.ChapterNumber;
            chapter.Title = request.Title;
            chapter.Content = request.Content;

            await _db.SaveChangesAsync();
            return NoContent();
        }

        [Authorize(Roles = "Admin")]
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> DeleteChapter(int id)
        {
            var chapter = await _db.Chapters.FindAsync(id);
            if (chapter == null)
                return NotFound();

            _db.Chapters.Remove(chapter);
            await _db.SaveChangesAsync();
            return NoContent();
        }
    }
}
