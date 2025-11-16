using LaboratorioAPI.Interfaces;
using LaboratorioAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace LaboratorioAPI.Repositories
{
    public class EquipoRepository : IEquipoRepository
    {
        private readonly List<Equipo> _equipos;
        private int _nextId = 1;

        public EquipoRepository()
        {
            _equipos = new List<Equipo>
            {
                new Equipo
                {
                    Id = _nextId++,
                    Nombre = "PC-01",
                    NumeroSerie = "SN001",
                    Marca = "Dell",
                    Modelo = "Optiplex 7070",
                    Estado = "Disponible",
                    Especificaciones = "Intel i7, 16GB RAM, 512GB SSD",
                    Ubicacion = "Laboratorio A",
                    FechaAdquisicion = new DateTime(2023, 1, 15),
                    Activo = true
                },
                new Equipo
                {
                    Id = _nextId++,
                    Nombre = "PC-02",
                    NumeroSerie = "SN002",
                    Marca = "HP",
                    Modelo = "ProDesk 600",
                    Estado = "Disponible",
                    Especificaciones = "Intel i5, 8GB RAM, 256GB SSD",
                    Ubicacion = "Laboratorio A",
                    FechaAdquisicion = new DateTime(2023, 2, 20),
                    Activo = true
                }
            };
        }

        public async Task<Equipo> GetByIdAsync(int id)
        {
            return await Task.FromResult(_equipos.FirstOrDefault(e => e.Id == id && e.Activo));
        }

        public async Task<IEnumerable<Equipo>> GetAllAsync()
        {
            return await Task.FromResult(_equipos.Where(e => e.Activo).ToList());
        }

        public async Task<Equipo> CreateAsync(Equipo equipo)
        {
            equipo.Id = _nextId++;
            equipo.Activo = true;
            _equipos.Add(equipo);
            return await Task.FromResult(equipo);
        }

        public async Task<Equipo> UpdateAsync(Equipo equipo)
        {
            var existingEquipo = _equipos.FirstOrDefault(e => e.Id == equipo.Id);
            if (existingEquipo != null)
            {
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
            }
            return await Task.FromResult(existingEquipo);
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var equipo = _equipos.FirstOrDefault(e => e.Id == id);
            if (equipo != null)
            {
                equipo.Activo = false;
                return await Task.FromResult(true);
            }
            return await Task.FromResult(false);
        }

        public async Task<bool> ExistsAsync(int id)
        {
            return await Task.FromResult(_equipos.Any(e => e.Id == id && e.Activo));
        }
    }
}