using LabProject.Application.DTOs;
using LabProject.Application.Interfaces;

namespace LabProject.Application.Services;

public class WeatherForecastService : IWeatherForecastService
{
    private readonly IWeatherForecastRepository _repository;

    public WeatherForecastService(IWeatherForecastRepository repository)
    {
        _repository = repository;
    }

    public async Task<IReadOnlyList<WeatherForecastDto>> GetForecastsAsync(CancellationToken cancellationToken = default)
    {
        var forecasts = await _repository.GetForecastsAsync(cancellationToken);

        return forecasts
            .Select(forecast => new WeatherForecastDto
            {
                Date = forecast.Date,
                Summary = forecast.Summary,
                TemperatureC = forecast.TemperatureC
            })
            .ToList();
    }
}
