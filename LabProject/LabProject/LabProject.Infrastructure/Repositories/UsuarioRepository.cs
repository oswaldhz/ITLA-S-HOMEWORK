using LabProject.Application.Interfaces;
using LabProject.Domain.Models;
using LabProject.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace LabProject.Infrastructure.Repositories;

public class UsuarioRepository : IUsuarioRepository
{
    private readonly LabProjectDbContext _context;

    public UsuarioRepository(LabProjectDbContext context)
    {
        _context = context;
    }

    public async Task<Usuario> AddAsync(Usuario usuario, CancellationToken cancellationToken = default)
    {
        _context.Usuarios.Add(usuario);
        await _context.SaveChangesAsync(cancellationToken);
        return usuario;
    }

    public async Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        var usuario = await _context.Usuarios.FindAsync(new object?[] { id }, cancellationToken);
        if (usuario == null)
        {
            return false;
        }

        _context.Usuarios.Remove(usuario);
        await _context.SaveChangesAsync(cancellationToken);
        return true;
    }

    public async Task<IReadOnlyList<Usuario>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _context.Usuarios
            .AsNoTracking()
            .OrderBy(u => u.Id)
            .ToListAsync(cancellationToken);
    }

    public async Task<Usuario?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        return await _context.Usuarios.FindAsync(new object?[] { id }, cancellationToken);
    }

    public async Task<Usuario?> GetByEmailAsync(string email, CancellationToken cancellationToken = default)
    {
        return await _context.Usuarios
            .AsNoTracking()
            .FirstOrDefaultAsync(u => u.Email == email, cancellationToken);
    }

    public async Task UpdateAsync(Usuario usuario, CancellationToken cancellationToken = default)
    {
        _context.Usuarios.Update(usuario);
        await _context.SaveChangesAsync(cancellationToken);
    }
}
