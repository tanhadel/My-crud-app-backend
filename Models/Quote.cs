namespace BookApi.Models;


/// Quote model representing a quote from a book.
/// Each quote belongs to a specific user and is linked to a book.
/// The author can be edited independently of the book's author.
  
public class Quote
{
    public int Id { get; set; } 
    public required string Text { get; set; }
    public required string Author { get; set; }
    public int PageNumber { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public int BookId { get; set; }
    // Foreign Key - ID of the user who created the quote
    public int UserId { get; set; }
    // Foreign Key - ID of the book the quote comes from
    public Book? Book { get; set; }
    // Foreign Key - ID of the user who created the quote
    public User? User { get; set; }
}
