using LabProject.Application.Interfaces;
using LabProject.Domain.Entities;
using LabProject.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace LabProject.Infrastructure.Repositories;

public class WeatherForecastRepository : IWeatherForecastRepository
{
    private static readonly string[] Summaries =
    [
        "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
    ];

    private readonly LabProjectDbContext _context;

    public WeatherForecastRepository(LabProjectDbContext context)
    {
        _context = context;
    }

    public async Task<IReadOnlyList<WeatherForecast>> GetForecastsAsync(CancellationToken cancellationToken = default)
    {
        if (!await _context.WeatherForecasts.AnyAsync(cancellationToken))
        {
            var forecasts = Enumerable.Range(1, 5)
                .Select(index => new WeatherForecast
                {
                    Date = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(index)),
                    TemperatureC = Random.Shared.Next(-20, 55),
                    Summary = Summaries[Random.Shared.Next(Summaries.Length)]
                });

            await _context.WeatherForecasts.AddRangeAsync(forecasts, cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);
        }

        return await _context.WeatherForecasts
            .AsNoTracking()
            .OrderBy(forecast => forecast.Date)
            .ToListAsync(cancellationToken);
    }
}
