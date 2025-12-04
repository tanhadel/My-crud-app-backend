using System.ComponentModel.DataAnnotations;

namespace BookApi.DTOs;


/// DTO for updating an existing quote.
/// All fields are optional - only provided fields are updated.
/// Used in PUT /api/quotes/{id}

public class UpdateQuoteDto
{
    [StringLength(2000)]
    public string? Text { get; set; }
    [Range(1, 10000)]
    public int? PageNumber { get; set; }
    public int? BookId { get; set; }
    [StringLength(200)]
    public string? Author { get; set; }
}
