namespace BookApi.Models;

// Book model representing a book in the system.
// Each book belongs to a specific user (UserId).

public class Book
{
    public int Id { get; set; } 
    public required string Title { get; set; }
    public required string Author { get; set; }
    public string? Description { get; set; }
    public string? Genre { get; set; }
    public int PublishedYear { get; set; }
    public string? ISBN { get; set; }
    public int UserId { get; set; }
    // Navigation property - reference to the user who owns the book.
    public User? User { get; set; }
}
