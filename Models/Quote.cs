namespace BookApi.Models;

public class Quote
{
    public int Id { get; set; }
    public required string Text { get; set; }
    public int PageNumber { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    
    // Foreign Keys
    public int BookId { get; set; }
    public int UserId { get; set; }
    
    // Navigation Properties
    public Book? Book { get; set; }
    public User? User { get; set; }
}
