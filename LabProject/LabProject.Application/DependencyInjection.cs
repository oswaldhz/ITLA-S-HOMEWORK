using LabProject.Application.Interfaces;
using LabProject.Application.Services;
using Microsoft.Extensions.DependencyInjection;

namespace LabProject.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddScoped<IEquipoService, EquipoService>();
        services.AddScoped<IUsuarioService, UsuarioService>();
        services.AddScoped<ISoftwareService, SoftwareService>();
        services.AddScoped<IReservaService, ReservaService>();
        services.AddScoped<IReservationService, ReservationService>();
        services.AddScoped<IAuthService, AuthService>();

        return services;
    }
}
