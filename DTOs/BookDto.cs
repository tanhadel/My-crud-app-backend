namespace BookApi.DTOs;


/// DTO for returning book information.
/// Used in GET /api/books and as response for create/update operations.

public class BookDto
{
    public int Id { get; set; }
    public required string Title { get; set; }
    public required string Author { get; set; }
    public string? Description { get; set; }
    public string? Genre { get; set; }
    public int PublishedYear { get; set; }
    public string? ISBN { get; set; }
}
