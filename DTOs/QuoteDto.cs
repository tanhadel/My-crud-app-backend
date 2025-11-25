namespace BookApi.DTOs;

public class QuoteDto
{
    public int Id { get; set; }
    public required string Text { get; set; }
    public int PageNumber { get; set; }
    public DateTime CreatedAt { get; set; }
    public int BookId { get; set; }
    public string? BookTitle { get; set; }
    public string? BookAuthor { get; set; }
}
