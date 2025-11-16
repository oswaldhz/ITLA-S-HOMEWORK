using LaboratorioAPI.Models;

namespace LaboratorioAPI.Interfaces
{
    public interface IEquipoRepository
    {
        Task<Equipo> GetByIdAsync(int id);
        Task<IEnumerable<Equipo>> GetAllAsync();
        Task<Equipo> CreateAsync(Equipo equipo);
        Task<Equipo> UpdateAsync(Equipo equipo);
        Task<bool> DeleteAsync(int id);
        Task<bool> ExistsAsync(int id);
    }
}