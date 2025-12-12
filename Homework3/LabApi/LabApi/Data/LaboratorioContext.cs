using LaboratorioAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace LaboratorioAPI.Data;

public class LaboratorioContext : DbContext
{
    public LaboratorioContext(DbContextOptions<LaboratorioContext> options) : base(options)
    {
    }

    public DbSet<Equipo> Equipos => Set<Equipo>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Equipo>(entity =>
        {
            entity.Property(e => e.Nombre)
                .IsRequired()
                .HasMaxLength(200);

            entity.Property(e => e.NumeroSerie)
                .IsRequired()
                .HasMaxLength(100);

            entity.HasIndex(e => e.NumeroSerie).IsUnique();

            entity.Property(e => e.Marca)
                .IsRequired()
                .HasMaxLength(100);

            entity.Property(e => e.Modelo)
                .IsRequired()
                .HasMaxLength(100);

            entity.Property(e => e.Estado)
                .IsRequired()
                .HasMaxLength(50)
                .HasDefaultValue("Disponible");

            entity.Property(e => e.Especificaciones)
                .HasMaxLength(1000);

            entity.Property(e => e.Ubicacion)
                .IsRequired()
                .HasMaxLength(200);

            entity.Property(e => e.Activo)
                .HasDefaultValue(true);
        });
    }
}
