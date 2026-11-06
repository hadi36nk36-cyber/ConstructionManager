using ConstructionManager.Api.Application.DTOs;
using ConstructionManager.Api.Domain.Enums;

namespace ConstructionManager.Api.Application.Interfaces.Services
{
    public interface ICustomerService
    {
        #region CRUD
        Task<List<CustomerDto>> GetAllAsync();
        Task AddAsync(CustomerDto dto);
        Task UpdateAsync(CustomerDto dto);
        Task DeleteAsync(int id);
        #endregion

        #region Search
        Task<CustomerDto?> SearchByCodeAsync(string code);
        Task<List<CustomerDto>> SearchByNameAsync(string name);
        Task<List<CustomerDto>> SearchByPhoneAsync(string phone);
        #endregion

        #region Filter
        Task<List<CustomerDto>> GetByTypeAsync(CustomerType type);
        #endregion
    }
}