using LaboratorioAPI.Interfaces;
using LaboratorioAPI.Models;
using LaboratorioAPI.DTOs;

namespace LaboratorioAPI.Services
{
    public class EquipoService : IEquipoService
    {
        private readonly IEquipoRepository _equipoRepository;

        public EquipoService(IEquipoRepository equipoRepository)
        {
            _equipoRepository = equipoRepository;
        }

        public async Task<EquipoDto> GetByIdAsync(int id)
        {
            var equipo = await _equipoRepository.GetByIdAsync(id);
            if (equipo == null) return null;

            return MapToDto(equipo);
        }

        public async Task<IEnumerable<EquipoDto>> GetAllAsync()
        {
            var equipos = await _equipoRepository.GetAllAsync();
            return equipos.Select(MapToDto);
        }

        public async Task<EquipoDto> CreateAsync(CreateEquipoDto createEquipoDto)
        {
            var equipo = new Equipo
            {
                Nombre = createEquipoDto.Nombre,
                NumeroSerie = createEquipoDto.NumeroSerie,
                Marca = createEquipoDto.Marca,
                Modelo = createEquipoDto.Modelo,
                Estado = "Disponible",
                Especificaciones = createEquipoDto.Especificaciones,
                Ubicacion = createEquipoDto.Ubicacion,
                FechaAdquisicion = createEquipoDto.FechaAdquisicion,
                Activo = true
            };

            var createdEquipo = await _equipoRepository.CreateAsync(equipo);
            return MapToDto(createdEquipo);
        }

        public async Task<EquipoDto> UpdateAsync(UpdateEquipoDto updateEquipoDto)
        {
            var equipoExists = await _equipoRepository.ExistsAsync(updateEquipoDto.Id);
            if (!equipoExists) return null;

            var equipo = new Equipo
            {
                Id = updateEquipoDto.Id,
                Nombre = updateEquipoDto.Nombre,
                NumeroSerie = updateEquipoDto.NumeroSerie,
                Marca = updateEquipoDto.Marca,
                Modelo = updateEquipoDto.Modelo,
                Estado = updateEquipoDto.Estado,
                Especificaciones = updateEquipoDto.Especificaciones,
                Ubicacion = updateEquipoDto.Ubicacion,
                FechaAdquisicion = updateEquipoDto.FechaAdquisicion,
                UltimoMantenimiento = updateEquipoDto.UltimoMantenimiento,
                Activo = updateEquipoDto.Activo
            };

            var updatedEquipo = await _equipoRepository.UpdateAsync(equipo);
            return MapToDto(updatedEquipo);
        }

        public async Task<bool> DeleteAsync(int id)
        {
            return await _equipoRepository.DeleteAsync(id);
        }

        private EquipoDto MapToDto(Equipo equipo)
        {
            return new EquipoDto
            {
                Id = equipo.Id,
                Nombre = equipo.Nombre,
                NumeroSerie = equipo.NumeroSerie,
                Marca = equipo.Marca,
                Modelo = equipo.Modelo,
                Estado = equipo.Estado,
                Especificaciones = equipo.Especificaciones,
                Ubicacion = equipo.Ubicacion,
                FechaAdquisicion = equipo.FechaAdquisicion,
                UltimoMantenimiento = equipo.UltimoMantenimiento,
                Activo = equipo.Activo
            };
        }
    }
}