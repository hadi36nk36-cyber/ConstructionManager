using ConstructionManager.Api.Application.DTOs;

namespace ConstructionManager.Api.Application.Interfaces.Services
{
    public interface IMaterialService
    {
        #region CRUD
        Task<List<MaterialDto>> GetAllAsync();
        Task AddAsync(MaterialDto dto);
        Task UpdateAsync(MaterialDto dto);
        Task DeleteAsync(int id);
        #endregion

        #region Search
        Task<MaterialDto?> SearchByCodeAsync(string code);
        Task<List<MaterialDto>> SearchByNameAsync(string name);
        #endregion

        #region Filter
        Task<List<MaterialDto>> GetByCategoryAsync(string category);
        #endregion
    }
}