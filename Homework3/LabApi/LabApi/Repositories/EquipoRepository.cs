using LaboratorioAPI.Data;
using LaboratorioAPI.Interfaces;
using LaboratorioAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace LaboratorioAPI.Repositories
{
    public class EquipoRepository : IEquipoRepository
    {
        private readonly LaboratorioContext _context;

        public EquipoRepository(LaboratorioContext context)
        {
            _context = context;
        }

        public async Task<Equipo?> GetByIdAsync(int id)
        {
            return await _context.Equipos
                .AsNoTracking()
                .FirstOrDefaultAsync(e => e.Id == id && e.Activo);
        }

        public async Task<IEnumerable<Equipo>> GetAllAsync()
        {
            return await _context.Equipos
                .AsNoTracking()
                .Where(e => e.Activo)
                .OrderBy(e => e.Id)
                .ToListAsync();
        }

        public async Task<Equipo> CreateAsync(Equipo equipo)
        {
            await _context.Equipos.AddAsync(equipo);
            await _context.SaveChangesAsync();
            return equipo;
        }

        public async Task<Equipo?> UpdateAsync(Equipo equipo)
        {
            var existingEquipo = await _context.Equipos.FirstOrDefaultAsync(e => e.Id == equipo.Id);
            if (existingEquipo == null)
            {
                return null;
            }

            existingEquipo.Nombre = equipo.Nombre;
            existingEquipo.NumeroSerie = equipo.NumeroSerie;
            existingEquipo.Marca = equipo.Marca;
            existingEquipo.Modelo = equipo.Modelo;
            existingEquipo.Estado = equipo.Estado;
            existingEquipo.Especificaciones = equipo.Especificaciones;
            existingEquipo.Ubicacion = equipo.Ubicacion;
            existingEquipo.FechaAdquisicion = equipo.FechaAdquisicion;
            existingEquipo.UltimoMantenimiento = equipo.UltimoMantenimiento;
            existingEquipo.Activo = equipo.Activo;

            await _context.SaveChangesAsync();
            return existingEquipo;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var equipo = await _context.Equipos.FirstOrDefaultAsync(e => e.Id == id);
            if (equipo == null)
            {
                return false;
            }

            equipo.Activo = false;
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> ExistsAsync(int id)
        {
            return await _context.Equipos.AnyAsync(e => e.Id == id && e.Activo);
        }

        public async Task<bool> SerialExistsAsync(string numeroSerie, int? excludingId = null)
        {
            return await _context.Equipos.AnyAsync(e =>
                e.Activo &&
                e.NumeroSerie.ToLower() == numeroSerie.ToLower() &&
                (!excludingId.HasValue || e.Id != excludingId.Value));
        }
    }
}