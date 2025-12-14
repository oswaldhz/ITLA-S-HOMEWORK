using LabProject.Domain.Models;

namespace LabProject.Application.Interfaces;

public interface IEquipoRepository
{
    Task<IReadOnlyList<Equipo>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<Equipo?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<bool> IdentificadorEnUsoAsync(string identificador, int? excludeId = null, CancellationToken cancellationToken = default);
    Task<Equipo> AddAsync(Equipo equipo, CancellationToken cancellationToken = default);
    Task UpdateAsync(Equipo equipo, CancellationToken cancellationToken = default);
    Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default);
}
