namespace BookApi.DTOs;

public class BookDto
{
    public int Id { get; set; }
    public required string Title { get; set; }
    public required string Author { get; set; }
    public string? Description { get; set; }
    public int PublishedYear { get; set; }
    public string? ISBN { get; set; }
}
