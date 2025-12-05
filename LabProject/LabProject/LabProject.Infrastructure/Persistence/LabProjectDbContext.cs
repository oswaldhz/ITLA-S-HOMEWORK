using LabProject.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace LabProject.Infrastructure.Persistence;

public class LabProjectDbContext : DbContext
{
    public LabProjectDbContext(DbContextOptions<LabProjectDbContext> options) : base(options)
    {
    }

    public DbSet<WeatherForecast> WeatherForecasts => Set<WeatherForecast>();
}
