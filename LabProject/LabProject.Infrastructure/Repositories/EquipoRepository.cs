using LabProject.Application.Interfaces;
using LabProject.Domain.Models;
using LabProject.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace LabProject.Infrastructure.Repositories;

public class EquipoRepository : IEquipoRepository
{
    private readonly LabProjectDbContext _context;

    public EquipoRepository(LabProjectDbContext context)
    {
        _context = context;
    }

    public async Task<Equipo> AddAsync(Equipo equipo, CancellationToken cancellationToken = default)
    {
        _context.Equipos.Add(equipo);
        await _context.SaveChangesAsync(cancellationToken);
        return equipo;
    }

    public async Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        var equipo = await _context.Equipos.FindAsync(new object?[] { id }, cancellationToken);
        if (equipo == null)
        {
            return false;
        }

        _context.Equipos.Remove(equipo);
        await _context.SaveChangesAsync(cancellationToken);
        return true;
    }

    public async Task<IReadOnlyList<Equipo>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _context.Equipos
            .AsNoTracking()
            .OrderBy(e => e.Id)
            .ToListAsync(cancellationToken);
    }

    public async Task<Equipo?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        return await _context.Equipos.FindAsync(new object?[] { id }, cancellationToken);
    }

    public async Task<bool> IdentificadorEnUsoAsync(string identificador, int? excludeId = null, CancellationToken cancellationToken = default)
    {
        return await _context.Equipos
            .AnyAsync(e => e.Identificador == identificador && (!excludeId.HasValue || e.Id != excludeId.Value), cancellationToken);
    }

    public async Task UpdateAsync(Equipo equipo, CancellationToken cancellationToken = default)
    {
        _context.Equipos.Update(equipo);
        await _context.SaveChangesAsync(cancellationToken);
    }
}
