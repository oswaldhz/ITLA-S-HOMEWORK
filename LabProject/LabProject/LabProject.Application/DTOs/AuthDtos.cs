using System.ComponentModel.DataAnnotations;

namespace LabProject.Application.DTOs;

public record AuthResponse(string Token, DateTime ExpiresAt, UserSessionDto User);

public record UserSessionDto(int Id, string Nombre, string Email, string Rol);

public class LoginRequest
{
    [Required]
    [EmailAddress]
    public string Email { get; set; } = string.Empty;

    [Required]
    [StringLength(200, MinimumLength = 6)]
    public string Password { get; set; } = string.Empty;
}
