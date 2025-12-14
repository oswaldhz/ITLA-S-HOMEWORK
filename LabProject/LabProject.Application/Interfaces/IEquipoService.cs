using LabProject.Application.DTOs;

namespace LabProject.Application.Interfaces;

public interface IEquipoService
{
    Task<IReadOnlyList<EquipoDto>> ObtenerTodosAsync(CancellationToken cancellationToken = default);
    Task<EquipoDto?> ObtenerPorIdAsync(int id, CancellationToken cancellationToken = default);
    Task<EquipoDto> CrearAsync(SaveEquipoRequest request, CancellationToken cancellationToken = default);
    Task<EquipoDto?> ActualizarAsync(int id, SaveEquipoRequest request, CancellationToken cancellationToken = default);
    Task<bool> EliminarAsync(int id, CancellationToken cancellationToken = default);
}
