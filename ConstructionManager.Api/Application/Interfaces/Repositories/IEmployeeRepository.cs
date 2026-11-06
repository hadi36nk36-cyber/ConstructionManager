using ConstructionManager.Api.Domain.Entities;
using ConstructionManager.Api.Domain.Enums;

namespace ConstructionManager.Api.Application.Interfaces.Repositories
{
    public interface IEmployeeRepository
    {
        #region CRUD
        Task<List<Employee>> GetAllAsync();
        Task AddAsync(Employee employee);
        Task UpdateAsync(Employee employee);
        Task DeleteAsync(int id);
        #endregion

        #region Search
        Task<List<Employee>> SearchByNameAsync(string name);
        Task<Employee?> SearchByNationalCodeAsync(string nationalCode);
        Task<Employee?> SearchByEmployeeCodeAsync(string employeeCode);
        #endregion

        #region Filter
        Task<List<Employee>> GetByStatusAsync(EmployeeStatus status);
        Task<List<Employee>> GetBySpecialityAsync(string speciality);
        #endregion

        #region Validation
        Task<bool> IsNationalCodeDuplicateAsync(string nationalCode);
        Task<bool> IsEmployeeCodeDuplicateAsync(string employeeCode);
        Task<bool> IsPhoneDuplicateAsync(string phone);
        #endregion

        #region Dependency
        Task<bool> HasProjectAssignmentsAsync(int employeeId);
        Task<bool> HasPaymentsAsync(int employeeId);
        #endregion
    }
}