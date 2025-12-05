using System.ComponentModel.DataAnnotations;

namespace LabProject.Application.DTOs;

public record EquipoDto(int Id, string Identificador, string Estado, string Ubicacion);

public class SaveEquipoRequest
{
    [Required]
    [StringLength(50)]
    public string Identificador { get; set; } = string.Empty;

    [Required]
    [StringLength(50)]
    public string Estado { get; set; } = string.Empty;

    [Required]
    [StringLength(100)]
    public string Ubicacion { get; set; } = string.Empty;
}
