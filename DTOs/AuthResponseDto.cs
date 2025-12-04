namespace BookApi.DTOs;

/// DTO for authentication response after successful registration/login.
/// Contains JWT token and user information.

public class AuthResponseDto
{
    public required string Token { get; set; }
    public required string Username { get; set; }
    public required string Email { get; set; }
}
