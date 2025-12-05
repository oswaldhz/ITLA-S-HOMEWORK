using LabProject.Application.Interfaces;
using LabProject.Infrastructure.Persistence;
using LabProject.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace LabProject.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services)
    {
        services.AddDbContext<LabProjectDbContext>(options => options.UseInMemoryDatabase("LabProjectDb"));
        services.AddScoped<IWeatherForecastRepository, WeatherForecastRepository>();

        return services;
    }
}
