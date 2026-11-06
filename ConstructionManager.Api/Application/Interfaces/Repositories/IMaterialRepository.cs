using ConstructionManager.Api.Domain.Entities;

namespace ConstructionManager.Api.Application.Interfaces.Repositories
{
    public interface IMaterialRepository
    {
        #region CRUD
        Task<List<Material>> GetAllAsync();
        Task AddAsync(Material material);
        Task UpdateAsync(Material material);
        Task DeleteAsync(int id);
        #endregion

        #region Search
        Task<Material?> SearchByCodeAsync(string code);
        Task<List<Material>> SearchByNameAsync(string name);
        #endregion

        #region Filter
        Task<List<Material>> GetByCategoryAsync(string category);
        #endregion

        #region Validation
        Task<bool> IsCodeDuplicateAsync(string code);
        #endregion
    }
}