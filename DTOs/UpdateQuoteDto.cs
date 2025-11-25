using System.ComponentModel.DataAnnotations;

namespace BookApi.DTOs;

public class UpdateQuoteDto
{
    [StringLength(2000)]
    public string? Text { get; set; }

    [Range(1, 10000)]
    public int? PageNumber { get; set; }
}
