using LabProject.Application.DTOs;
using LabProject.Application.Interfaces;
using LabProject.Domain.Models;

namespace LabProject.Application.Services;

public class UsuarioService : IUsuarioService
{
    private readonly IUsuarioRepository _usuarioRepository;

    public UsuarioService(IUsuarioRepository usuarioRepository)
    {
        _usuarioRepository = usuarioRepository;
    }

    public async Task<UsuarioDto> CrearAsync(SaveUsuarioRequest request, CancellationToken cancellationToken = default)
    {
        var usuario = new Usuario
        {
            Nombre = request.Nombre,
            Email = request.Email,
            Rol = request.Rol
        };

        usuario.Validate();

        var created = await _usuarioRepository.AddAsync(usuario, cancellationToken);
        return Map(created);
    }

    public async Task<UsuarioDto?> ActualizarAsync(int id, SaveUsuarioRequest request, CancellationToken cancellationToken = default)
    {
        var usuario = await _usuarioRepository.GetByIdAsync(id, cancellationToken);
        if (usuario == null)
        {
            return null;
        }

        usuario.Nombre = request.Nombre;
        usuario.Email = request.Email;
        usuario.Rol = request.Rol;
        usuario.Validate();

        await _usuarioRepository.UpdateAsync(usuario, cancellationToken);
        return Map(usuario);
    }

    public async Task<bool> EliminarAsync(int id, CancellationToken cancellationToken = default)
    {
        return await _usuarioRepository.DeleteAsync(id, cancellationToken);
    }

    public async Task<UsuarioDto?> ObtenerPorIdAsync(int id, CancellationToken cancellationToken = default)
    {
        var usuario = await _usuarioRepository.GetByIdAsync(id, cancellationToken);
        return usuario == null ? null : Map(usuario);
    }

    public async Task<IReadOnlyList<UsuarioDto>> ObtenerTodosAsync(CancellationToken cancellationToken = default)
    {
        var usuarios = await _usuarioRepository.GetAllAsync(cancellationToken);
        return usuarios.Select(Map).ToList();
    }

    private static UsuarioDto Map(Usuario usuario) => new(usuario.Id, usuario.Nombre, usuario.Email, usuario.Rol);
}
