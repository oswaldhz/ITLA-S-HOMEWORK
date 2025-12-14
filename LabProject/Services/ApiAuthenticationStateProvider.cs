using System.Security.Claims;
using LabProject.Application.DTOs;
using Microsoft.AspNetCore.Components.Authorization;

namespace LabProject.Services;

public class ApiAuthenticationStateProvider : AuthenticationStateProvider
{
    private readonly AuthState _authState;

    public ApiAuthenticationStateProvider(AuthState authState)
    {
        _authState = authState;
        _authState.SessionChanged += NotifyAuthenticationStateChangedInternal;
    }

    public override async Task<AuthenticationState> GetAuthenticationStateAsync()
    {
        await _authState.EnsureInitializedAsync();
        var principal = CreatePrincipal(_authState.Session);
        return new AuthenticationState(principal);
    }

    private static ClaimsPrincipal CreatePrincipal(AuthResponse? session)
    {
        if (session is null || session.ExpiresAt <= DateTime.UtcNow)
        {
            return new ClaimsPrincipal(new ClaimsIdentity());
        }

        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, session.User.Id.ToString()),
            new(ClaimTypes.Name, session.User.Nombre),
            new(ClaimTypes.Email, session.User.Email),
            new(ClaimTypes.Role, session.User.Rol)
        };

        var identity = new ClaimsIdentity(claims, authenticationType: "apiauth");
        return new ClaimsPrincipal(identity);
    }

    private void NotifyAuthenticationStateChangedInternal()
    {
        NotifyAuthenticationStateChanged(GetAuthenticationStateAsync());
    }
}
