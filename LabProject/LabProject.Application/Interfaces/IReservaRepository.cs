using LabProject.Domain.Models;

namespace LabProject.Application.Interfaces;

public interface IReservaRepository
{
    Task<Reserva?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Reserva>> GetByFiltersAsync(DateTime? fecha, int? equipoId, int? usuarioId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Reserva>> GetReservasDeEquipoEnRangoAsync(int equipoId, DateTime inicio, DateTime fin, CancellationToken cancellationToken = default);
    Task<Reserva> AddAsync(Reserva reserva, CancellationToken cancellationToken = default);
    Task<Reserva?> UpdateAsync(int id, int usuarioId, int equipoId, DateTime fechaInicio, DateTime fechaFin, IEnumerable<int> softwareIds, CancellationToken cancellationToken = default);
    Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default);
}
