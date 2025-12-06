using LabProject.Application.Interfaces;
using LabProject.Infrastructure.Persistence;
using LabProject.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace LabProject.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<LabProjectDbContext>(options =>
            options.UseSqlServer(configuration.GetConnectionString("DefaultConnection")));
        services.AddScoped<IEquipoRepository, EquipoRepository>();
        services.AddScoped<IUsuarioRepository, UsuarioRepository>();
        services.AddScoped<ISoftwareRepository, SoftwareRepository>();
        services.AddScoped<IReservaRepository, ReservaRepository>();

        return services;
    }
}
