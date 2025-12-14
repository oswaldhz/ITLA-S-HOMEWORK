using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using LabProject.Application.DTOs;

namespace LabProject.Services;

public class ApiClient
{
    public const string HttpClientName = "Api";

    private readonly IHttpClientFactory _httpClientFactory;
    private readonly AuthState _authState;

    public ApiClient(IHttpClientFactory httpClientFactory, AuthState authState)
    {
        _httpClientFactory = httpClientFactory;
        _authState = authState;
    }

    public async Task<AuthResponse?> LoginAsync(LoginRequest request, CancellationToken cancellationToken = default)
    {
        var client = _httpClientFactory.CreateClient(HttpClientName);
        var response = await client.PostAsJsonAsync("api/Auth/login", request, cancellationToken);

        if (response.IsSuccessStatusCode)
        {
            return await response.Content.ReadFromJsonAsync<AuthResponse>(cancellationToken: cancellationToken);
        }

        if (response.StatusCode == HttpStatusCode.Unauthorized)
        {
            return null;
        }

        response.EnsureSuccessStatusCode();
        return null;
    }

    public async Task<T?> GetAsync<T>(string requestUri, CancellationToken cancellationToken = default)
    {
        var client = await ConfigureClientAsync();
        var response = await client.GetAsync(requestUri, cancellationToken);
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<T>(cancellationToken: cancellationToken);
    }

    public async Task<TResponse?> PostAsync<TRequest, TResponse>(string requestUri, TRequest payload, CancellationToken cancellationToken = default)
    {
        var client = await ConfigureClientAsync();
        var response = await client.PostAsJsonAsync(requestUri, payload, cancellationToken);
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<TResponse>(cancellationToken: cancellationToken);
    }

    public async Task<TResponse?> PutAsync<TRequest, TResponse>(string requestUri, TRequest payload, CancellationToken cancellationToken = default)
    {
        var client = await ConfigureClientAsync();
        var response = await client.PutAsJsonAsync(requestUri, payload, cancellationToken);
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<TResponse>(cancellationToken: cancellationToken);
    }

    public async Task DeleteAsync(string requestUri, CancellationToken cancellationToken = default)
    {
        var client = await ConfigureClientAsync();
        var response = await client.DeleteAsync(requestUri, cancellationToken);
        response.EnsureSuccessStatusCode();
    }

    private async Task<HttpClient> ConfigureClientAsync()
    {
        await _authState.EnsureInitializedAsync();
        var client = _httpClientFactory.CreateClient(HttpClientName);

        if (_authState.Session is { Token: not null } session && session.ExpiresAt > DateTime.UtcNow)
        {
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", session.Token);
        }
        else
        {
            client.DefaultRequestHeaders.Authorization = null;
        }

        return client;
    }
}
