using System.ComponentModel.DataAnnotations;

namespace BookApi.DTOs;

// DTO for creating a new book.
// Used in POST /api/books

public class CreateBookDto
{
    [Required]
    [StringLength(200)]
    public required string Title { get; set; }
    [Required]
    [StringLength(100)]
    public required string Author { get; set; }
    [StringLength(1000)]
    public string? Description { get; set; }
    [StringLength(100)]
    public string? Genre { get; set; }
    [Range(1000, 2100)]
    public int PublishedYear { get; set; }
    [StringLength(20)]
    public string? ISBN { get; set; }
}
