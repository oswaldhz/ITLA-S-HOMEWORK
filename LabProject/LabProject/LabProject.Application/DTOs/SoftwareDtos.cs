using System.ComponentModel.DataAnnotations;

namespace LabProject.Application.DTOs;

public record SoftwareDto(int Id, string Nombre, string Version, string Licencia);

public class SaveSoftwareRequest
{
    [Required]
    [StringLength(100)]
    public string Nombre { get; set; } = string.Empty;

    [Required]
    [StringLength(50)]
    public string Version { get; set; } = string.Empty;

    [Required]
    [StringLength(50)]
    public string Licencia { get; set; } = string.Empty;
}
