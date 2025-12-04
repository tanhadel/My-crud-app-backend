using System.ComponentModel.DataAnnotations;

namespace BookApi.DTOs;


/// DTO for creating a new quote.
/// Used in POST /api/quotes

public class CreateQuoteDto
{
    [Required]
    [StringLength(2000)]
    public required string Text { get; set; }
    [Required]
    [StringLength(200)]
    public required string Author { get; set; }
    [Range(1, 10000)]
    public int PageNumber { get; set; }
    [Required]
    public int BookId { get; set; }
}
