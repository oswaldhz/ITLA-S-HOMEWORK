using LabProject.Application.Interfaces;
using LabProject.Application.Services;
using Microsoft.Extensions.DependencyInjection;

namespace LabProject.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddScoped<IWeatherForecastService, WeatherForecastService>();
        services.AddScoped<IEquipoService, EquipoService>();
        services.AddScoped<IUsuarioService, UsuarioService>();
        services.AddScoped<ISoftwareService, SoftwareService>();
        services.AddScoped<IReservaService, ReservaService>();

        return services;
    }
}
