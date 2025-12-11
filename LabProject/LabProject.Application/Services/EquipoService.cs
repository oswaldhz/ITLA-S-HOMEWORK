using LabProject.Application.DTOs;
using LabProject.Application.Interfaces;
using LabProject.Domain.Models;

namespace LabProject.Application.Services;

public class EquipoService : IEquipoService
{
    private readonly IEquipoRepository _equipoRepository;

    public EquipoService(IEquipoRepository equipoRepository)
    {
        _equipoRepository = equipoRepository;
    }

    public async Task<EquipoDto> CrearAsync(SaveEquipoRequest request, CancellationToken cancellationToken = default)
    {
        var equipo = new Equipo
        {
            Identificador = request.Identificador,
            Estado = request.Estado,
            Ubicacion = request.Ubicacion
        };

        equipo.Validate();

        if (await _equipoRepository.IdentificadorEnUsoAsync(equipo.Identificador, null, cancellationToken))
        {
            throw new InvalidOperationException("Ya existe un equipo con el mismo identificador.");
        }

        var created = await _equipoRepository.AddAsync(equipo, cancellationToken);
        return Map(created);
    }

    public async Task<EquipoDto?> ActualizarAsync(int id, SaveEquipoRequest request, CancellationToken cancellationToken = default)
    {
        var equipo = await _equipoRepository.GetByIdAsync(id, cancellationToken);
        if (equipo == null)
        {
            return null;
        }

        equipo.Identificador = request.Identificador;
        equipo.Estado = request.Estado;
        equipo.Ubicacion = request.Ubicacion;

        equipo.Validate();

        if (await _equipoRepository.IdentificadorEnUsoAsync(equipo.Identificador, id, cancellationToken))
        {
            throw new InvalidOperationException("Ya existe un equipo con el mismo identificador.");
        }

        await _equipoRepository.UpdateAsync(equipo, cancellationToken);
        return Map(equipo);
    }

    public async Task<bool> EliminarAsync(int id, CancellationToken cancellationToken = default)
    {
        return await _equipoRepository.DeleteAsync(id, cancellationToken);
    }

    public async Task<EquipoDto?> ObtenerPorIdAsync(int id, CancellationToken cancellationToken = default)
    {
        var equipo = await _equipoRepository.GetByIdAsync(id, cancellationToken);
        return equipo == null ? null : Map(equipo);
    }

    public async Task<IReadOnlyList<EquipoDto>> ObtenerTodosAsync(CancellationToken cancellationToken = default)
    {
        var equipos = await _equipoRepository.GetAllAsync(cancellationToken);
        return equipos.Select(Map).ToList();
    }

    private static EquipoDto Map(Equipo equipo) => new(equipo.Id, equipo.Identificador, equipo.Estado, equipo.Ubicacion);
}
