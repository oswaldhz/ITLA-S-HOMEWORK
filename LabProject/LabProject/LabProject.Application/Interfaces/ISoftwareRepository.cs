using LabProject.Domain.Models;

namespace LabProject.Application.Interfaces;

public interface ISoftwareRepository
{
    Task<IReadOnlyList<Software>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<Software?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<Software> AddAsync(Software software, CancellationToken cancellationToken = default);
    Task UpdateAsync(Software software, CancellationToken cancellationToken = default);
    Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Software>> GetByIdsAsync(IEnumerable<int> ids, CancellationToken cancellationToken = default);
}
