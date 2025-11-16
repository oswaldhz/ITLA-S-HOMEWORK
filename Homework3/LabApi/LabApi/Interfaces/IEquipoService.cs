using LaboratorioAPI.Models;
using LaboratorioAPI.DTOs;

namespace LaboratorioAPI.Interfaces
{
    public interface IEquipoService
    {
        Task<EquipoDto> GetByIdAsync(int id);
        Task<IEnumerable<EquipoDto>> GetAllAsync();
        Task<EquipoDto> CreateAsync(CreateEquipoDto createEquipoDto);
        Task<EquipoDto> UpdateAsync(UpdateEquipoDto updateEquipoDto);
        Task<bool> DeleteAsync(int id);
    }
}