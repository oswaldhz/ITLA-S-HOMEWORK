using LaboratorioAPI.DTOs;
using LaboratorioAPI.Exceptions;
using LaboratorioAPI.Interfaces;
using LaboratorioAPI.Models;

namespace LaboratorioAPI.Services
{
    public class EquipoService : IEquipoService
    {
        private readonly IEquipoRepository _equipoRepository;

        public EquipoService(IEquipoRepository equipoRepository)
        {
            _equipoRepository = equipoRepository;
        }

        public async Task<EquipoDto?> GetByIdAsync(int id)
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
            ValidateEquipo(createEquipoDto);
            await ValidateSerialUniquenessAsync(createEquipoDto.NumeroSerie, null);

            var equipo = new Equipo
            {
                Nombre = createEquipoDto.Nombre.Trim(),
                NumeroSerie = createEquipoDto.NumeroSerie.Trim(),
                Marca = createEquipoDto.Marca.Trim(),
                Modelo = createEquipoDto.Modelo.Trim(),
                Estado = "Disponible",
                Especificaciones = createEquipoDto.Especificaciones,
                Ubicacion = createEquipoDto.Ubicacion.Trim(),
                FechaAdquisicion = createEquipoDto.FechaAdquisicion,
                Activo = true
            };

            var createdEquipo = await _equipoRepository.CreateAsync(equipo);
            return MapToDto(createdEquipo);
        }

        public async Task<EquipoDto?> UpdateAsync(UpdateEquipoDto updateEquipoDto)
        {
            ValidateEquipo(updateEquipoDto);
            await ValidateSerialUniquenessAsync(updateEquipoDto.NumeroSerie, updateEquipoDto.Id);

            var equipoExists = await _equipoRepository.ExistsAsync(updateEquipoDto.Id);
            if (!equipoExists) return null;

            var equipo = new Equipo
            {
                Id = updateEquipoDto.Id,
                Nombre = updateEquipoDto.Nombre.Trim(),
                NumeroSerie = updateEquipoDto.NumeroSerie.Trim(),
                Marca = updateEquipoDto.Marca.Trim(),
                Modelo = updateEquipoDto.Modelo.Trim(),
                Estado = updateEquipoDto.Estado,
                Especificaciones = updateEquipoDto.Especificaciones,
                Ubicacion = updateEquipoDto.Ubicacion.Trim(),
                FechaAdquisicion = updateEquipoDto.FechaAdquisicion,
                UltimoMantenimiento = updateEquipoDto.UltimoMantenimiento,
                Activo = updateEquipoDto.Activo
            };

            var updatedEquipo = await _equipoRepository.UpdateAsync(equipo);
            return updatedEquipo == null ? null : MapToDto(updatedEquipo);
        }

        public async Task<bool> DeleteAsync(int id)
        {
            return await _equipoRepository.DeleteAsync(id);
        }

        public async Task<EquipoDto> ReserveAsync(int id)
        {
            var equipo = await _equipoRepository.GetByIdAsync(id);
            if (equipo == null)
            {
                throw new DomainValidationException($"Equipo con ID {id} no encontrado");
            }

            if (equipo.Estado.Equals("Reservado", StringComparison.OrdinalIgnoreCase))
            {
                throw new ReservationConflictException($"El equipo con ID {id} ya está reservado");
            }

            equipo.Estado = "Reservado";
            var updatedEquipo = await _equipoRepository.UpdateAsync(equipo)
                ?? throw new DomainValidationException($"No se pudo actualizar el equipo con ID {id}");

            return MapToDto(updatedEquipo);
        }

        private void ValidateEquipo(CreateEquipoDto createEquipoDto)
        {
            if (string.IsNullOrWhiteSpace(createEquipoDto.Nombre))
            {
                throw new DomainValidationException("El nombre del equipo es obligatorio");
            }

            if (string.IsNullOrWhiteSpace(createEquipoDto.NumeroSerie))
            {
                throw new DomainValidationException("El número de serie del equipo es obligatorio");
            }

            if (string.IsNullOrWhiteSpace(createEquipoDto.Marca))
            {
                throw new DomainValidationException("La marca del equipo es obligatoria");
            }

            if (string.IsNullOrWhiteSpace(createEquipoDto.Modelo))
            {
                throw new DomainValidationException("El modelo del equipo es obligatorio");
            }

            if (string.IsNullOrWhiteSpace(createEquipoDto.Ubicacion))
            {
                throw new DomainValidationException("La ubicación del equipo es obligatoria");
            }
        }

        private void ValidateEquipo(UpdateEquipoDto updateEquipoDto)
        {
            if (updateEquipoDto.Id <= 0)
            {
                throw new DomainValidationException("El ID del equipo debe ser mayor que cero");
            }

            ValidateEquipo(new CreateEquipoDto
            {
                Nombre = updateEquipoDto.Nombre,
                NumeroSerie = updateEquipoDto.NumeroSerie,
                Marca = updateEquipoDto.Marca,
                Modelo = updateEquipoDto.Modelo,
                Ubicacion = updateEquipoDto.Ubicacion
            });
        }

        private async Task ValidateSerialUniquenessAsync(string numeroSerie, int? excludingId)
        {
            if (await _equipoRepository.SerialExistsAsync(numeroSerie, excludingId))
            {
                throw new DomainValidationException($"Ya existe un equipo con el número de serie {numeroSerie}");
            }
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
                Especificaciones = string.IsNullOrWhiteSpace(equipo.Especificaciones)
                    ? "Pendiente de registrar"
                    : equipo.Especificaciones,
                Ubicacion = equipo.Ubicacion,
                FechaAdquisicion = equipo.FechaAdquisicion,
                UltimoMantenimiento = equipo.UltimoMantenimiento,
                Activo = equipo.Activo
            };
        }
    }
}
