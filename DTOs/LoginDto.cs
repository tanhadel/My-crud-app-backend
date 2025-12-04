using System.ComponentModel.DataAnnotations;

namespace BookApi.DTOs;

/// DTO for user login.
/// Used when an existing user logs in.
public class LoginDto
{
    [Required]
    [EmailAddress]
    public required string Email { get; set; }
    [Required]
    public required string Password { get; set; }
}
