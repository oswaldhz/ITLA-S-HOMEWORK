using LabProject.Domain.Entities;
using LabProject.Domain.Models;
using Microsoft.EntityFrameworkCore;

namespace LabProject.Infrastructure.Persistence;

public class LabProjectDbContext : DbContext
{
    public LabProjectDbContext(DbContextOptions<LabProjectDbContext> options) : base(options)
    {
    }

    public DbSet<Usuario> Usuarios => Set<Usuario>();
    public DbSet<Equipo> Equipos => Set<Equipo>();
    public DbSet<Software> Softwares => Set<Software>();
    public DbSet<Reserva> Reservas => Set<Reserva>();
    public DbSet<ReservaSoftware> ReservasSoftware => Set<ReservaSoftware>();
    public DbSet<WeatherForecast> WeatherForecasts => Set<WeatherForecast>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Usuario>(entity =>
        {
            entity.Property(u => u.Nombre)
                .IsRequired()
                .HasMaxLength(100);

            entity.Property(u => u.Email)
                .IsRequired()
                .HasMaxLength(200);

            entity.Property(u => u.Rol)
                .IsRequired()
                .HasMaxLength(50);
        });

        modelBuilder.Entity<Equipo>(entity =>
        {
            entity.Property(e => e.Identificador)
                .IsRequired()
                .HasMaxLength(50);

            entity.Property(e => e.Estado)
                .IsRequired()
                .HasMaxLength(50);

            entity.Property(e => e.Ubicacion)
                .IsRequired()
                .HasMaxLength(100);

            entity.HasIndex(e => e.Identificador).IsUnique();

            entity.HasData(
                new Equipo { Id = 1, Identificador = "PC-001", Estado = "Disponible", Ubicacion = "Sala A" },
                new Equipo { Id = 2, Identificador = "PC-002", Estado = "Mantenimiento", Ubicacion = "Sala B" }
            );
        });

        modelBuilder.Entity<Software>(entity =>
        {
            entity.Property(s => s.Nombre)
                .IsRequired()
                .HasMaxLength(100);

            entity.Property(s => s.Version)
                .IsRequired()
                .HasMaxLength(50);

            entity.Property(s => s.Licencia)
                .IsRequired()
                .HasMaxLength(50);

            entity.HasData(
                new Software { Id = 1, Nombre = "Visual Studio Code", Version = "1.90", Licencia = "MIT" },
                new Software { Id = 2, Nombre = "SQL Server Management Studio", Version = "19.3", Licencia = "Free" }
            );
        });

        modelBuilder.Entity<Reserva>(entity =>
        {
            entity.Property(r => r.FechaInicio).IsRequired();
            entity.Property(r => r.FechaFin).IsRequired();

            entity.HasOne<Usuario>()
                .WithMany()
                .HasForeignKey(r => r.UsuarioId)
                .OnDelete(DeleteBehavior.Cascade)
                .IsRequired();

            entity.HasOne<Equipo>()
                .WithMany()
                .HasForeignKey(r => r.EquipoId)
                .OnDelete(DeleteBehavior.Restrict)
                .IsRequired();
        });

        modelBuilder.Entity<ReservaSoftware>(entity =>
        {
            entity.HasKey(rs => new { rs.ReservaId, rs.SoftwareId });

            entity.HasOne(rs => rs.Reserva)
                .WithMany(r => r.ReservaSoftware)
                .HasForeignKey(rs => rs.ReservaId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(rs => rs.Software)
                .WithMany()
                .HasForeignKey(rs => rs.SoftwareId)
                .OnDelete(DeleteBehavior.Cascade);
        });
    }
}
