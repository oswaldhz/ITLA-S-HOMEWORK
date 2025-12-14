using LabProject.Application.Interfaces;
using LabProject.Domain.Models;
using LabProject.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace LabProject.Infrastructure.Repositories;

public class ReservaRepository : IReservaRepository
{
    private readonly LabProjectDbContext _context;

    public ReservaRepository(LabProjectDbContext context)
    {
        _context = context;
    }

    public async Task<Reserva> AddAsync(Reserva reserva, CancellationToken cancellationToken = default)
    {
        _context.Reservas.Add(reserva);
        await _context.SaveChangesAsync(cancellationToken);
        return reserva;
    }

    public async Task<Reserva?> UpdateAsync(
        int id,
        int usuarioId,
        int equipoId,
        DateTime fechaInicio,
        DateTime fechaFin,
        IEnumerable<int> softwareIds,
        CancellationToken cancellationToken = default)
    {
        var reserva = await _context.Reservas
            .Include(r => r.ReservaSoftware)
            .FirstOrDefaultAsync(r => r.Id == id, cancellationToken);

        if (reserva == null)
        {
            return null;
        }

        reserva.UsuarioId = usuarioId;
        reserva.EquipoId = equipoId;
        reserva.Programar(fechaInicio, fechaFin);

        // Reemplazar software asociados.
        if (reserva.ReservaSoftware.Any())
        {
            _context.ReservasSoftware.RemoveRange(reserva.ReservaSoftware);
            reserva.ReservaSoftware.Clear();
        }

        foreach (var softwareId in softwareIds?.Distinct() ?? Enumerable.Empty<int>())
        {
            reserva.ReservaSoftware.Add(new ReservaSoftware
            {
                ReservaId = reserva.Id,
                SoftwareId = softwareId
            });
        }

        await _context.SaveChangesAsync(cancellationToken);
        return reserva;
    }

    public async Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        var reserva = await _context.Reservas
            .Include(r => r.ReservaSoftware)
            .FirstOrDefaultAsync(r => r.Id == id, cancellationToken);

        if (reserva == null)
        {
            return false;
        }

        _context.Reservas.Remove(reserva);
        await _context.SaveChangesAsync(cancellationToken);
        return true;
    }

    public async Task<IReadOnlyList<Reserva>> GetByFiltersAsync(DateTime? fecha, int? equipoId, int? usuarioId, CancellationToken cancellationToken = default)
    {
        var query = _context.Reservas
            .Include(r => r.ReservaSoftware)
            .AsQueryable();

        if (fecha.HasValue)
        {
            var targetDate = fecha.Value.Date;
            query = query.Where(r => r.FechaInicio.Date <= targetDate && r.FechaFin.Date >= targetDate);
        }

        if (equipoId.HasValue)
        {
            query = query.Where(r => r.EquipoId == equipoId.Value);
        }

        if (usuarioId.HasValue)
        {
            query = query.Where(r => r.UsuarioId == usuarioId.Value);
        }

        return await query
            .AsNoTracking()
            .OrderBy(r => r.FechaInicio)
            .ToListAsync(cancellationToken);
    }

    public async Task<Reserva?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        return await _context.Reservas
            .Include(r => r.ReservaSoftware)
            .AsNoTracking()
            .FirstOrDefaultAsync(r => r.Id == id, cancellationToken);
    }

    public async Task<IReadOnlyList<Reserva>> GetReservasDeEquipoEnRangoAsync(int equipoId, DateTime inicio, DateTime fin, CancellationToken cancellationToken = default)
    {
        return await _context.Reservas
            .Where(r => r.EquipoId == equipoId && r.FechaInicio < fin && r.FechaFin > inicio)
            .AsNoTracking()
            .ToListAsync(cancellationToken);
    }
}
