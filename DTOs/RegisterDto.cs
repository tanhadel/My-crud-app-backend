using System.ComponentModel.DataAnnotations;

namespace BookApi.DTOs;

/// <summary>
/// DTO for registering a new user.
/// Used when a new user creates an account.
/// </summary>
public class RegisterDto
{
    [Required]
    [StringLength(50, MinimumLength = 3)]
    public required string Username { get; set; }
    [Required]
    [EmailAddress]
    public required string Email { get; set; }
    [Required]
    [StringLength(100, MinimumLength = 6)]
    public required string Password { get; set; }
}
