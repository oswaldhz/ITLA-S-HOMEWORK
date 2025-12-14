using LabProject.Application.DTOs;

namespace LabProject.Application.Interfaces;

public interface ISoftwareService
{
    Task<IReadOnlyList<SoftwareDto>> ObtenerTodosAsync(CancellationToken cancellationToken = default);
    Task<SoftwareDto?> ObtenerPorIdAsync(int id, CancellationToken cancellationToken = default);
    Task<SoftwareDto> CrearAsync(SaveSoftwareRequest request, CancellationToken cancellationToken = default);
    Task<SoftwareDto?> ActualizarAsync(int id, SaveSoftwareRequest request, CancellationToken cancellationToken = default);
    Task<bool> EliminarAsync(int id, CancellationToken cancellationToken = default);
}
