using System.ComponentModel.DataAnnotations;

namespace BookApi.DTOs;

public class CreateQuoteDto
{
    [Required]
    [StringLength(2000)]
    public required string Text { get; set; }

    [Range(1, 10000)]
    public int PageNumber { get; set; }

    [Required]
    public int BookId { get; set; }
}
