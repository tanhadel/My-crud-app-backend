using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using BookApi.Data;
using BookApi.DTOs;
using BookApi.Models;

namespace BookApi.Controllers;

// Controller for managing books.
// All endpoints require authentication and users can only view/modify their own books.

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class BooksController : ControllerBase
{
    private readonly ApplicationDbContext _context;

    public BooksController(ApplicationDbContext context)
    {
        _context = context;
    }


    // Gets the user ID from the logged-in user's JWT token.
    // The user's ID
    private int GetUserId()
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        return int.Parse(userIdClaim ?? "0");
    }

    /// Gets all books belonging to the logged-in user.
    /// List of user's books
    [HttpGet]
    public async Task<ActionResult<IEnumerable<BookDto>>> GetBooks()
    {
        var userId = GetUserId();

        var books = await _context.Books
            .Where(b => b.UserId == userId)
            .Select(b => new BookDto
            {
                Id = b.Id,
                Title = b.Title,
                Author = b.Author,
                Description = b.Description,
                Genre = b.Genre,
                PublishedYear = b.PublishedYear,
                ISBN = b.ISBN
            })
            .ToListAsync();

        return Ok(books);
    }

    // Gets a specific book by ID.
    /// Gets a specific book by ID.
    /// User can only retrieve books they own.
    /// </summary>
    /// <param name="id">Book ID</param>
    /// <returns>The book if found and belongs to user, otherwise 404 Not Found</returns>
    [HttpGet("{id}")]
    public async Task<ActionResult<BookDto>> GetBook(int id)
    {
        var userId = GetUserId();

        var book = await _context.Books
            .FirstOrDefaultAsync(b => b.Id == id && b.UserId == userId);

        if (book == null)
        {
            return NotFound(new { message = "Book not found" });
        }

        var bookDto = new BookDto
        {
            Id = book.Id,
            Title = book.Title,
            Author = book.Author,
            Description = book.Description,
            Genre = book.Genre,
            PublishedYear = book.PublishedYear,
            ISBN = book.ISBN
        };

        return Ok(bookDto);
    }


    /// Creates a new book for the logged-in user.
    /// The book is automatically linked to the user's ID.
    ///
    ///  createBookDto" Data for the new book (Title and Author are required)
    /// The newly created book with generated ID
    [HttpPost]
    public async Task<ActionResult<BookDto>> CreateBook(CreateBookDto createBookDto)
    {
        var userId = GetUserId();

        var book = new Book
        {
            Title = createBookDto.Title,
            Author = createBookDto.Author,
            Description = createBookDto.Description,
            Genre = createBookDto.Genre,
            PublishedYear = createBookDto.PublishedYear,
            ISBN = createBookDto.ISBN,
            UserId = userId 
        };

        _context.Books.Add(book);
        await _context.SaveChangesAsync();

        var bookDto = new BookDto
        {
            Id = book.Id,
            Title = book.Title,
            Author = book.Author,
            Description = book.Description,
            Genre = book.Genre,
            PublishedYear = book.PublishedYear,
            ISBN = book.ISBN
        };

        return CreatedAtAction(nameof(GetBook), new { id = book.Id }, bookDto);
    }


    /// Updates an existing book.
    /// User can only update books they own.
    /// All fields in UpdateBookDto are optional - only provided fields are updated.

    //id Book ID
    /// param name="updateBookDto" Fields to update
    /// returns 204 No Content if update succeeds, 404 Not Found if book not found.
    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateBook(int id, UpdateBookDto updateBookDto)
    {
        var userId = GetUserId();

        var book = await _context.Books
            .FirstOrDefaultAsync(b => b.Id == id && b.UserId == userId);

        if (book == null)
        {
            return NotFound(new { message = "Book not found" });
        }

        // Update only provided fields
        if (updateBookDto.Title != null)
            book.Title = updateBookDto.Title;
        if (updateBookDto.Author != null)
            book.Author = updateBookDto.Author;
        if (updateBookDto.Description != null)
            book.Description = updateBookDto.Description;
        if (updateBookDto.Genre != null)
            book.Genre = updateBookDto.Genre;
        if (updateBookDto.PublishedYear.HasValue)
            book.PublishedYear = updateBookDto.PublishedYear.Value;
        if (updateBookDto.ISBN != null)
            book.ISBN = updateBookDto.ISBN;

        await _context.SaveChangesAsync();

        return NoContent();
    }


    /// Deletes a book.
    /// User can only delete books they own.
    //id Book ID
    // returns 204 No Content if deletion succeeds, 404 Not Found if book not found
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteBook(int id)
    {
        var userId = GetUserId();

        var book = await _context.Books
            .FirstOrDefaultAsync(b => b.Id == id && b.UserId == userId);

        if (book == null)
        {
            return NotFound(new { message = "Book not found" });
        }

        _context.Books.Remove(book);
        await _context.SaveChangesAsync();

        return NoContent();
    }
}
