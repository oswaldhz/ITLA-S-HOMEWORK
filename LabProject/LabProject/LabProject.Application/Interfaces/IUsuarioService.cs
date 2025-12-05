using LabProject.Application.DTOs;

namespace LabProject.Application.Interfaces;

public interface IUsuarioService
{
    Task<IReadOnlyList<UsuarioDto>> ObtenerTodosAsync(CancellationToken cancellationToken = default);
    Task<UsuarioDto?> ObtenerPorIdAsync(int id, CancellationToken cancellationToken = default);
    Task<UsuarioDto> CrearAsync(SaveUsuarioRequest request, CancellationToken cancellationToken = default);
    Task<UsuarioDto?> ActualizarAsync(int id, SaveUsuarioRequest request, CancellationToken cancellationToken = default);
    Task<bool> EliminarAsync(int id, CancellationToken cancellationToken = default);
}
