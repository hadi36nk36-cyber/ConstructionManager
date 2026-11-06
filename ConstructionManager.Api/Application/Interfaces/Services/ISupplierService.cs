using ConstructionManager.Api.Application.DTOs;

namespace ConstructionManager.Api.Application.Interfaces.Services
{
    public interface ISupplierService
    {
        #region CRUD
        Task<List<SupplierDto>> GetAllAsync();
        Task AddAsync(SupplierDto dto);
        Task UpdateAsync(SupplierDto dto);
        Task DeleteAsync(int id);
        #endregion

        #region Search
        Task<SupplierDto?> SearchByCodeAsync(string code);
        Task<List<SupplierDto>> SearchByNameAsync(string name);
        Task<List<SupplierDto>> SearchByPhoneAsync(string phone);
        #endregion
    }
}