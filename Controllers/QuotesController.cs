using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using BookApi.Data;
using BookApi.DTOs;
using BookApi.Models;

namespace BookApi.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class QuotesController : ControllerBase
{
    private readonly ApplicationDbContext _context;

    public QuotesController(ApplicationDbContext context)
    {
        _context = context;
    }

    private int GetUserId()
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        return int.Parse(userIdClaim ?? "0");
    }

    // GET: api/quotes - Get all quotes for the authenticated user
    [HttpGet]
    public async Task<ActionResult<IEnumerable<QuoteDto>>> GetMyQuotes()
    {
        var userId = GetUserId();

        var quotes = await _context.Quotes
            .Include(q => q.Book)
            .Where(q => q.UserId == userId)
            .Select(q => new QuoteDto
            {
                Id = q.Id,
                Text = q.Text,
                PageNumber = q.PageNumber,
                CreatedAt = q.CreatedAt,
                BookId = q.BookId,
                BookTitle = q.Book != null ? q.Book.Title : null,
                BookAuthor = q.Book != null ? q.Book.Author : null
            })
            .ToListAsync();

        return Ok(quotes);
    }

    // GET: api/quotes/5 - Get a specific quote (only if it belongs to the user)
    [HttpGet("{id}")]
    public async Task<ActionResult<QuoteDto>> GetQuote(int id)
    {
        var userId = GetUserId();

        var quote = await _context.Quotes
            .Include(q => q.Book)
            .FirstOrDefaultAsync(q => q.Id == id && q.UserId == userId);

        if (quote == null)
        {
            return NotFound(new { message = "Quote not found" });
        }

        var quoteDto = new QuoteDto
        {
            Id = quote.Id,
            Text = quote.Text,
            PageNumber = quote.PageNumber,
            CreatedAt = quote.CreatedAt,
            BookId = quote.BookId,
            BookTitle = quote.Book?.Title,
            BookAuthor = quote.Book?.Author
        };

        return Ok(quoteDto);
    }

    // GET: api/quotes/book/5 - Get all quotes for a specific book (only user's quotes)
    [HttpGet("book/{bookId}")]
    public async Task<ActionResult<IEnumerable<QuoteDto>>> GetQuotesByBook(int bookId)
    {
        var userId = GetUserId();

        var quotes = await _context.Quotes
            .Include(q => q.Book)
            .Where(q => q.BookId == bookId && q.UserId == userId)
            .Select(q => new QuoteDto
            {
                Id = q.Id,
                Text = q.Text,
                PageNumber = q.PageNumber,
                CreatedAt = q.CreatedAt,
                BookId = q.BookId,
                BookTitle = q.Book != null ? q.Book.Title : null,
                BookAuthor = q.Book != null ? q.Book.Author : null
            })
            .ToListAsync();

        return Ok(quotes);
    }

    // POST: api/quotes
    [HttpPost]
    public async Task<ActionResult<QuoteDto>> CreateQuote(CreateQuoteDto createQuoteDto)
    {
        var userId = GetUserId();

        // Verify that the book exists
        var bookExists = await _context.Books.AnyAsync(b => b.Id == createQuoteDto.BookId);
        if (!bookExists)
        {
            return BadRequest(new { message = "Book not found" });
        }

        var quote = new Quote
        {
            Text = createQuoteDto.Text,
            PageNumber = createQuoteDto.PageNumber,
            BookId = createQuoteDto.BookId,
            UserId = userId,
            CreatedAt = DateTime.UtcNow
        };

        _context.Quotes.Add(quote);
        await _context.SaveChangesAsync();

        // Load the book information
        await _context.Entry(quote).Reference(q => q.Book).LoadAsync();

        var quoteDto = new QuoteDto
        {
            Id = quote.Id,
            Text = quote.Text,
            PageNumber = quote.PageNumber,
            CreatedAt = quote.CreatedAt,
            BookId = quote.BookId,
            BookTitle = quote.Book?.Title,
            BookAuthor = quote.Book?.Author
        };

        return CreatedAtAction(nameof(GetQuote), new { id = quote.Id }, quoteDto);
    }

    // PUT: api/quotes/5
    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateQuote(int id, UpdateQuoteDto updateQuoteDto)
    {
        var userId = GetUserId();

        var quote = await _context.Quotes
            .FirstOrDefaultAsync(q => q.Id == id && q.UserId == userId);

        if (quote == null)
        {
            return NotFound(new { message = "Quote not found" });
        }

        // Update only provided fields
        if (updateQuoteDto.Text != null)
            quote.Text = updateQuoteDto.Text;
        if (updateQuoteDto.PageNumber.HasValue)
            quote.PageNumber = updateQuoteDto.PageNumber.Value;

        await _context.SaveChangesAsync();

        return NoContent();
    }

    // DELETE: api/quotes/5
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteQuote(int id)
    {
        var userId = GetUserId();

        var quote = await _context.Quotes
            .FirstOrDefaultAsync(q => q.Id == id && q.UserId == userId);

        if (quote == null)
        {
            return NotFound(new { message = "Quote not found" });
        }

        _context.Quotes.Remove(quote);
        await _context.SaveChangesAsync();

        return NoContent();
    }
}
