using System.ComponentModel.DataAnnotations;

namespace LabProject.Application.DTOs;

public record ReservaDto(
    int Id,
    int UsuarioId,
    string UsuarioNombre,
    int EquipoId,
    string EquipoIdentificador,
    DateTime FechaInicio,
    DateTime FechaFin,
    IReadOnlyCollection<int> SoftwareIds);

public class CreateReservaRequest
{
    [Required]
    public int UsuarioId { get; set; }

    [Required]
    public int EquipoId { get; set; }

    [Required]
    public DateTime FechaInicio { get; set; }

    [Required]
    public DateTime FechaFin { get; set; }

    public ICollection<int> SoftwareIds { get; set; } = new List<int>();
}
