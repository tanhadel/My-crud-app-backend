using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using BookApi.Data;
using BookApi.DTOs;
using BookApi.Models;

namespace BookApi.Controllers;


/// Controller for managing quotes from books.
/// All endpoints require authentication and users can only view/modify their own quotes.
/// Quotes have an independent Author field that can be manually edited.

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

    /// Gets the user ID from the logged-in user's JWT token.
    /// The user's ID
    private int GetUserId()
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        return int.Parse(userIdClaim ?? "0");
    }

    /// Gets all quotes belonging to the logged-in user.
    /// Includes the book title for each quote.
    /// List of user's quotes
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
                Author = q.Author,
                PageNumber = q.PageNumber,
                CreatedAt = q.CreatedAt,
                BookId = q.BookId,
                BookTitle = q.Book != null ? q.Book.Title : null
            })
            .ToListAsync();

        return Ok(quotes);
    }

    // Gets a specific quote by ID.
    // User can only retrieve quotes they own.
    // id Quote ID
    // The quote if found and belongs to user, otherwise 404 Not Found
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
            Author = quote.Author,
            PageNumber = quote.PageNumber,
            CreatedAt = quote.CreatedAt,
            BookId = quote.BookId,
            BookTitle = quote.Book?.Title
        };

        return Ok(quoteDto);
    }


    /// Gets all quotes for a specific book.
    /// User can only retrieve their own quotes from the specified book.
   
    ///  bookId Book ID
    /// List of quotes from the specified book
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
                Author = q.Author,
                PageNumber = q.PageNumber,
                CreatedAt = q.CreatedAt,
                BookId = q.BookId,
                BookTitle = q.Book != null ? q.Book.Title : null
            })
            .ToListAsync();

        return Ok(quotes);
    }
    // Creates a new quote for the logged-in user.
    // Creates a new quote for the logged-in user.
    // The author field is independent and can be manually edited.
    // The quote is automatically linked to the user's ID.
    
    // createQuoteDto  Data for the new quote (Text, Author, and BookId are required)</param>
    // The newly created quote with generated IDs>
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
            Author = createQuoteDto.Author,
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
            Author = quote.Author,
            PageNumber = quote.PageNumber,
            CreatedAt = quote.CreatedAt,
            BookId = quote.BookId,
            BookTitle = quote.Book?.Title
        };

        return CreatedAtAction(nameof(GetQuote), new { id = quote.Id }, quoteDto);
    }

    // Updates an existing quote.
    // User can only update quotes they own.
    // All fields in UpdateQuoteDto are optional - only provided fields are updated.
    // The author field can be changed independently of the book's author.
    // id Quote ID
    // updateQuoteDto Fields to update
    // 204 No Content if update succeeds, 404 Not Found if quote not found
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
        if (updateQuoteDto.Author != null)
            quote.Author = updateQuoteDto.Author;
        if (updateQuoteDto.PageNumber.HasValue)
            quote.PageNumber = updateQuoteDto.PageNumber.Value;

        await _context.SaveChangesAsync();

        return NoContent();
    }

                

    // Deletes a quote.
    // User can only delete quotes they own.
    // id Quote ID
    // 204 No Content if deletion succeeds, 404 Not Found if quote not found
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
