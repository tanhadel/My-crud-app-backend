using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BookApi.Data;
using BookApi.DTOs;
using BookApi.Models;

namespace BookApi.Controllers;

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

    // GET: api/books
    [HttpGet]
    public async Task<ActionResult<IEnumerable<BookDto>>> GetBooks()
    {
        var books = await _context.Books
            .Select(b => new BookDto
            {
                Id = b.Id,
                Title = b.Title,
                Author = b.Author,
                Description = b.Description,
                PublishedYear = b.PublishedYear,
                ISBN = b.ISBN
            })
            .ToListAsync();

        return Ok(books);
    }

    // GET: api/books/5
    [HttpGet("{id}")]
    public async Task<ActionResult<BookDto>> GetBook(int id)
    {
        var book = await _context.Books.FindAsync(id);

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
            PublishedYear = book.PublishedYear,
            ISBN = book.ISBN
        };

        return Ok(bookDto);
    }

    // POST: api/books
    [HttpPost]
    public async Task<ActionResult<BookDto>> CreateBook(CreateBookDto createBookDto)
    {
        var book = new Book
        {
            Title = createBookDto.Title,
            Author = createBookDto.Author,
            Description = createBookDto.Description,
            PublishedYear = createBookDto.PublishedYear,
            ISBN = createBookDto.ISBN
        };

        _context.Books.Add(book);
        await _context.SaveChangesAsync();

        var bookDto = new BookDto
        {
            Id = book.Id,
            Title = book.Title,
            Author = book.Author,
            Description = book.Description,
            PublishedYear = book.PublishedYear,
            ISBN = book.ISBN
        };

        return CreatedAtAction(nameof(GetBook), new { id = book.Id }, bookDto);
    }

    // PUT: api/books/5
    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateBook(int id, UpdateBookDto updateBookDto)
    {
        var book = await _context.Books.FindAsync(id);

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
        if (updateBookDto.PublishedYear.HasValue)
            book.PublishedYear = updateBookDto.PublishedYear.Value;
        if (updateBookDto.ISBN != null)
            book.ISBN = updateBookDto.ISBN;

        await _context.SaveChangesAsync();

        return NoContent();
    }

    // DELETE: api/books/5
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteBook(int id)
    {
        var book = await _context.Books.FindAsync(id);

        if (book == null)
        {
            return NotFound(new { message = "Book not found" });
        }

        _context.Books.Remove(book);
        await _context.SaveChangesAsync();

        return NoContent();
    }
}
