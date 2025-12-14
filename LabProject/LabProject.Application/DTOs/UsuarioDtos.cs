using System.ComponentModel.DataAnnotations;

namespace LabProject.Application.DTOs;

public record UsuarioDto(int Id, string Nombre, string Email, string Rol);

public class SaveUsuarioRequest
{
    [Required]
    [StringLength(100)]
    public string Nombre { get; set; } = string.Empty;

    [Required]
    [EmailAddress]
    [StringLength(200)]
    public string Email { get; set; } = string.Empty;

    [Required]
    [StringLength(50)]
    public string Rol { get; set; } = string.Empty;

    [StringLength(200, MinimumLength = 6)]
    public string? Password { get; set; }
}
