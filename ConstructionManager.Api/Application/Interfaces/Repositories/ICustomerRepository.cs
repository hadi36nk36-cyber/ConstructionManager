using ConstructionManager.Api.Domain.Entities;
using ConstructionManager.Api.Domain.Enums;

namespace ConstructionManager.Api.Application.Interfaces.Repositories
{
    public interface ICustomerRepository
    {
        #region CRUD
        Task<List<Customer>> GetAllAsync();
        Task AddAsync(Customer customer);
        Task UpdateAsync(Customer customer);
        Task DeleteAsync(int id);
        #endregion

        #region Search
        Task<Customer?> SearchByCodeAsync(string code);
        Task<List<Customer>> SearchByNameAsync(string name);
        Task<List<Customer>> SearchByPhoneAsync(string phone);
        #endregion

        #region Filter
        Task<List<Customer>> GetByTypeAsync(CustomerType type);
        #endregion

        #region Validation
        Task<bool> IsCodeDuplicateAsync(string code);
        Task<bool> IsPhoneDuplicateAsync(string phone);
        #endregion

        #region Dependency
        Task<bool> HasProjectsAsync(int customerId);
        Task<bool> HasInvoicesAsync(int customerId);
        #endregion
    }
}