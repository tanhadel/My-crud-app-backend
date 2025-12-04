using System.ComponentModel.DataAnnotations;

namespace BookApi.DTOs;
/// DTO for updating an existing book.
/// All fields are optional - only provided fields are updated.
/// Used in PUT /api/books/{id}
    /// </summary>
public class UpdateBookDto
{
    [StringLength(200)]
    public string? Title { get; set; }
    [StringLength(100)]
    public string? Author { get; set; }
    [StringLength(1000)]
    public string? Description { get; set; }
    [StringLength(100)]
    public string? Genre { get; set; }
    [Range(1000, 2100)]
    public int? PublishedYear { get; set; }
    [StringLength(20)]
    public string? ISBN { get; set; }
}
