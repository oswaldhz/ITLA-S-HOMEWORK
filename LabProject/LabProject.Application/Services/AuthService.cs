using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using LabProject.Application.DTOs;
using LabProject.Application.Interfaces;
using LabProject.Application.Options;
using LabProject.Application.Security;
using LabProject.Domain.Models;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace LabProject.Application.Services;

public class AuthService : IAuthService
{
    private readonly IUsuarioRepository _usuarioRepository;
    private readonly JwtSettings _jwtSettings;

    public AuthService(IUsuarioRepository usuarioRepository, IOptions<JwtSettings> jwtOptions)
    {
        _usuarioRepository = usuarioRepository;
        _jwtSettings = jwtOptions.Value;
    }

    public async Task<AuthResponse?> LoginAsync(LoginRequest request, CancellationToken cancellationToken = default)
    {
        var usuario = await _usuarioRepository.GetByEmailAsync(request.Email, cancellationToken);
        if (usuario == null)
        {
            return null;
        }

        if (!PasswordHasher.Verify(request.Password, usuario.PasswordHash))
        {
            return null;
        }

        var expiresAt = DateTime.UtcNow.AddMinutes(_jwtSettings.ExpirationMinutes);
        var claims = new List<Claim>
        {
            new(JwtRegisteredClaimNames.Sub, usuario.Id.ToString()),
            new(JwtRegisteredClaimNames.Email, usuario.Email),
            new(ClaimTypes.Name, usuario.Nombre),
            new(ClaimTypes.Role, usuario.Rol)
        };

        var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.SecretKey));
        var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: _jwtSettings.Issuer,
            audience: _jwtSettings.Audience,
            claims: claims,
            expires: expiresAt,
            signingCredentials: credentials);

        var tokenString = new JwtSecurityTokenHandler().WriteToken(token);

        return new AuthResponse(
            tokenString,
            expiresAt,
            new UserSessionDto(usuario.Id, usuario.Nombre, usuario.Email, usuario.Rol));
    }
}
