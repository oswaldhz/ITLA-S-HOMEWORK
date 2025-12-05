using LabProject.Application.Interfaces;
using LabProject.Domain.Models;
using LabProject.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace LabProject.Infrastructure.Repositories;

public class SoftwareRepository : ISoftwareRepository
{
    private readonly LabProjectDbContext _context;

    public SoftwareRepository(LabProjectDbContext context)
    {
        _context = context;
    }

    public async Task<Software> AddAsync(Software software, CancellationToken cancellationToken = default)
    {
        _context.Softwares.Add(software);
        await _context.SaveChangesAsync(cancellationToken);
        return software;
    }

    public async Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        var software = await _context.Softwares.FindAsync(new object?[] { id }, cancellationToken);
        if (software == null)
        {
            return false;
        }

        _context.Softwares.Remove(software);
        await _context.SaveChangesAsync(cancellationToken);
        return true;
    }

    public async Task<IReadOnlyList<Software>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _context.Softwares
            .AsNoTracking()
            .OrderBy(s => s.Id)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<Software>> GetByIdsAsync(IEnumerable<int> ids, CancellationToken cancellationToken = default)
    {
        var idList = ids.ToList();
        if (idList.Count == 0)
        {
            return new List<Software>();
        }

        return await _context.Softwares
            .Where(s => idList.Contains(s.Id))
            .ToListAsync(cancellationToken);
    }

    public async Task<Software?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        return await _context.Softwares.FindAsync(new object?[] { id }, cancellationToken);
    }

    public async Task UpdateAsync(Software software, CancellationToken cancellationToken = default)
    {
        _context.Softwares.Update(software);
        await _context.SaveChangesAsync(cancellationToken);
    }
}
