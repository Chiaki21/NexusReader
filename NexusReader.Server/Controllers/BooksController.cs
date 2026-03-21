using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NexusReader.Server.Data;
using NexusReader.Shared.Models;

namespace NexusReader.Server.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class BooksController : ControllerBase
    {
        private readonly AppDbContext _context;

        public BooksController(AppDbContext context)
        {
            _context = context;
        }

        // GET: api/books
        [HttpGet]
        public async Task<ActionResult<IEnumerable<BookModel>>> GetBooks()
        {
            return await _context.Books.Include(b => b.Chapters).ToListAsync();
        }

        // POST: api/books
        [HttpPost]
        public async Task<ActionResult<BookModel>> PostBook(BookModel book)
        {
            _context.Books.Add(book);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetBooks), new { id = book.Id }, book);
        }
    }
}