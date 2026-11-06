using ConstructionManager.Api.Domain.Entities;

namespace ConstructionManager.Api.Application.Interfaces.Repositories
{
    public interface ISupplierRepository
    {
        #region CRUD
        Task<List<Supplier>> GetAllAsync();
        Task AddAsync(Supplier supplier);
        Task UpdateAsync(Supplier supplier);
        Task DeleteAsync(int id);
        #endregion

        #region Search
        Task<Supplier?> SearchByCodeAsync(string code);
        Task<List<Supplier>> SearchByNameAsync(string name);
        Task<List<Supplier>> SearchByPhoneAsync(string phone);
        #endregion

        #region Validation
        Task<bool> IsCodeDuplicateAsync(string code);
        Task<bool> IsPhoneDuplicateAsync(string phone);
        #endregion

        #region Dependency
        Task<bool> HasPurchaseInvoicesAsync(int supplierId);
        #endregion
    }
}