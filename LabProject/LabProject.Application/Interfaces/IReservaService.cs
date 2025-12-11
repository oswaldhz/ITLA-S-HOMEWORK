using LabProject.Application.DTOs;

namespace LabProject.Application.Interfaces;

public interface IReservaService
{
    Task<ReservaDto> CrearAsync(CreateReservaRequest request, ReservationValidationResult? validationResult = null, CancellationToken cancellationToken = default);
    Task<bool> CancelarAsync(int id, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<ReservaDto>> ObtenerPorFiltrosAsync(DateTime? fecha, int? equipoId, int? usuarioId, CancellationToken cancellationToken = default);
    Task<ReservaDto?> ObtenerPorIdAsync(int id, CancellationToken cancellationToken = default);
}
