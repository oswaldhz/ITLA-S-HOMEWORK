using LabProject.Application.DTOs;
using LabProject.Application.Interfaces;
using LabProject.Domain.Models;

namespace LabProject.Application.Services;

public class SoftwareService : ISoftwareService
{
    private readonly ISoftwareRepository _softwareRepository;

    public SoftwareService(ISoftwareRepository softwareRepository)
    {
        _softwareRepository = softwareRepository;
    }

    public async Task<SoftwareDto> CrearAsync(SaveSoftwareRequest request, CancellationToken cancellationToken = default)
    {
        var software = new Software
        {
            Nombre = request.Nombre,
            Version = request.Version,
            Licencia = request.Licencia
        };

        software.Validate();

        var created = await _softwareRepository.AddAsync(software, cancellationToken);
        return Map(created);
    }

    public async Task<SoftwareDto?> ActualizarAsync(int id, SaveSoftwareRequest request, CancellationToken cancellationToken = default)
    {
        var software = await _softwareRepository.GetByIdAsync(id, cancellationToken);
        if (software == null)
        {
            return null;
        }

        software.Nombre = request.Nombre;
        software.Version = request.Version;
        software.Licencia = request.Licencia;
        software.Validate();

        await _softwareRepository.UpdateAsync(software, cancellationToken);
        return Map(software);
    }

    public async Task<bool> EliminarAsync(int id, CancellationToken cancellationToken = default)
    {
        return await _softwareRepository.DeleteAsync(id, cancellationToken);
    }

    public async Task<SoftwareDto?> ObtenerPorIdAsync(int id, CancellationToken cancellationToken = default)
    {
        var software = await _softwareRepository.GetByIdAsync(id, cancellationToken);
        return software == null ? null : Map(software);
    }

    public async Task<IReadOnlyList<SoftwareDto>> ObtenerTodosAsync(CancellationToken cancellationToken = default)
    {
        var softwares = await _softwareRepository.GetAllAsync(cancellationToken);
        return softwares.Select(Map).ToList();
    }

    private static SoftwareDto Map(Software software) => new(software.Id, software.Nombre, software.Version, software.Licencia);
}
