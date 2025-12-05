using LaboratorioAPI.DTOs;
using LaboratorioAPI.Exceptions;
using LaboratorioAPI.Repositories;
using LaboratorioAPI.Services;

namespace LaboratorioAPI.Tests
{
    public class EquipoServiceTests
    {
        [Fact]
        public async Task CreateAsync_ThrowsDomainValidationException_WhenNombreIsEmpty()
        {
            var repository = new EquipoRepository();
            var service = new EquipoService(repository);
            var dto = new CreateEquipoDto
            {
                Nombre = " ",
                NumeroSerie = "SN003",
                Marca = "HP",
                Modelo = "Envy",
                Ubicacion = "Laboratorio B",
                FechaAdquisicion = DateTime.UtcNow
            };

            await Assert.ThrowsAsync<DomainValidationException>(() => service.CreateAsync(dto));
        }

        [Fact]
        public async Task CreateAsync_ThrowsDomainValidationException_WhenSerialIsDuplicated()
        {
            var repository = new EquipoRepository();
            var service = new EquipoService(repository);

            var initialDto = new CreateEquipoDto
            {
                Nombre = "PC-03",
                NumeroSerie = "SN010",
                Marca = "Dell",
                Modelo = "Latitude",
                Ubicacion = "Laboratorio B",
                FechaAdquisicion = DateTime.UtcNow
            };

            await service.CreateAsync(initialDto);

            var duplicateDto = new CreateEquipoDto
            {
                Nombre = "PC-04",
                NumeroSerie = "SN010",
                Marca = "Dell",
                Modelo = "Latitude",
                Ubicacion = "Laboratorio C",
                FechaAdquisicion = DateTime.UtcNow
            };

            await Assert.ThrowsAsync<DomainValidationException>(() => service.CreateAsync(duplicateDto));
        }

        [Fact]
        public async Task ReserveAsync_ThrowsReservationConflictException_WhenAlreadyReserved()
        {
            var repository = new EquipoRepository();
            var service = new EquipoService(repository);
            var created = await service.CreateAsync(new CreateEquipoDto
            {
                Nombre = "PC-05",
                NumeroSerie = "SN020",
                Marca = "Lenovo",
                Modelo = "ThinkPad",
                Ubicacion = "Laboratorio D",
                FechaAdquisicion = DateTime.UtcNow
            });

            await service.ReserveAsync(created.Id);

            await Assert.ThrowsAsync<ReservationConflictException>(() => service.ReserveAsync(created.Id));
        }
    }
}
