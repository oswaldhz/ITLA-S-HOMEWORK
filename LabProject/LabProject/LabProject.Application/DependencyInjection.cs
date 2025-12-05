using LabProject.Application.Interfaces;
using LabProject.Application.Services;
using Microsoft.Extensions.DependencyInjection;

namespace LabProject.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddScoped<IWeatherForecastService, WeatherForecastService>();

        return services;
    }
}
