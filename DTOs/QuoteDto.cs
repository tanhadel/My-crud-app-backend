namespace BookApi.DTOs;

/// DTO for returning quote information.
/// Used in GET /api/quotes and as response for create/update operations.

public class QuoteDto
{
    public int Id { get; set; }
    public required string Text { get; set; }
    public required string Author { get; set; }
    public int PageNumber { get; set; }
    public DateTime CreatedAt { get; set; }
    public int BookId { get; set; }
    public string? BookTitle { get; set; }
}
