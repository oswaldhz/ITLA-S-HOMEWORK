using LabProject.Domain.Entities;

namespace LabProject.Application.Interfaces;

public interface IWeatherForecastRepository
{
    Task<IReadOnlyList<WeatherForecast>> GetForecastsAsync(CancellationToken cancellationToken = default);
}
