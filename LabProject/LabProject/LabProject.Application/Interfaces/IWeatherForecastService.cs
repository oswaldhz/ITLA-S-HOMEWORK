using LabProject.Application.DTOs;

namespace LabProject.Application.Interfaces;

public interface IWeatherForecastService
{
    Task<IReadOnlyList<WeatherForecastDto>> GetForecastsAsync(CancellationToken cancellationToken = default);
}
