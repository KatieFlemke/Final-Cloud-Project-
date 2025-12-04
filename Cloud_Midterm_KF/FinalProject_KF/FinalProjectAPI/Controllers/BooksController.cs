using FinalProjectAPI.Data;
using FinalProjectAPI.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FinalProjectAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class BooksController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly ILogger<BooksController> _logger;

        public BooksController(AppDbContext context, ILogger<BooksController> logger)
        {
            _context = context;
            _logger = logger;
        }

        // GET: api/books
        [HttpGet]
        public async Task<IActionResult> GetBooks()
        {
            var books = await _context.Books.ToListAsync();
            return Ok(books);
        }

        // GET: api/books/{id}
        [HttpGet("{id}")]
        public async Task<IActionResult> GetBook(int id)
        {
            var book = await _context.Books.FindAsync(id);
            if (book == null) return NotFound(new { error = "Book not found" });
            return Ok(book);
        }

        // POST: api/books
        [HttpPost]
        public async Task<IActionResult> CreateBook(Book book)
        {
            if (string.IsNullOrWhiteSpace(book.Title) || string.IsNullOrWhiteSpace(book.Author))
                return BadRequest(new { error = "Title and Author are required" });

            _context.Books.Add(book);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetBook), new { id = book.Id }, book);
        }

        // PUT: api/books/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateBook(int id, Book updated)
        {
            if (id != updated.Id)
                return BadRequest(new { error = "ID mismatch" });

            if (string.IsNullOrWhiteSpace(updated.Title) || string.IsNullOrWhiteSpace(updated.Author))
                return BadRequest(new { error = "Title and Author are required" });

            _context.Entry(updated).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            return NoContent();
        }

        // DELETE: api/books/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteBook(int id)
        {
            var book = await _context.Books.FindAsync(id);
            if (book == null) return NotFound(new { error = "Book not found" });

            _context.Books.Remove(book);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        // PATCH: api/books/validate
        // Entity-specific validation rule:
        // If Year < (current year - 10), mark as Archived and set LastValidated timestamp
        [HttpPatch("validate")]
        public async Task<IActionResult> ValidateBooks()
        {
            var books = await _context.Books.ToListAsync();
            var updatedCount = 0;
            var now = DateTime.UtcNow;
            var currentYear = now.Year;

            foreach (var book in books)
            {
                if (book.Year < currentYear - 10 && !book.Archived)
                {
                    book.Archived = true;
                    book.LastValidated = now;
                    updatedCount++;
                }
            }

            await _context.SaveChangesAsync();

            _logger.LogInformation("ValidationTriggered - updated {Count} books", updatedCount);

            return Ok(new
            {
                updatedCount,
                timestamp = now.ToString("o")
            });
        }
    }
}
