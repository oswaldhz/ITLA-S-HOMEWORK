using LabProject.Application.DTOs;
using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;

namespace LabProject.Services;

public class AuthState
{
    private const string SessionStorageKey = "authSession";

    private readonly ProtectedLocalStorage _storage;

    private AuthResponse? _session;
    private bool _initialized;

    public AuthState(ProtectedLocalStorage storage)
    {
        _storage = storage;
    }

    public event Action? SessionChanged;

    public AuthResponse? Session => IsAuthenticated ? _session : null;

    public bool IsAuthenticated => _session?.ExpiresAt > DateTime.UtcNow;

    public bool IsAdmin => string.Equals(_session?.User.Rol, "Admin", StringComparison.OrdinalIgnoreCase);

    public async Task EnsureInitializedAsync()
    {
        if (_initialized)
        {
            return;
        }

        var storedSession = await _storage.GetAsync<AuthResponse>(SessionStorageKey);
        if (storedSession.Success && storedSession.Value?.ExpiresAt > DateTime.UtcNow)
        {
            _session = storedSession.Value;
        }
        else if (storedSession.Success)
        {
            await _storage.DeleteAsync(SessionStorageKey);
        }

        _initialized = true;
    }

    public async Task SetSessionAsync(AuthResponse response)
    {
        _session = response;
        _initialized = true;

        await _storage.SetAsync(SessionStorageKey, response);
        SessionChanged?.Invoke();
    }

    public async Task LogoutAsync()
    {
        _session = null;
        _initialized = true;

        await _storage.DeleteAsync(SessionStorageKey);
        SessionChanged?.Invoke();
    }
}
