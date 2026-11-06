using ConstructionManager.Api.Application.DTOs;
using ConstructionManager.Api.Domain.Enums;

namespace ConstructionManager.Api.Application.Interfaces.Services
{
    public interface IEmployeeService
    {
        #region CRUD
        Task<List<EmployeeDto>> GetAllAsync();
        Task AddAsync(EmployeeDto dto);
        Task UpdateAsync(EmployeeDto dto);
        Task DeleteAsync(int id);
        #endregion

        #region Search
        Task<List<EmployeeDto>> SearchByNameAsync(string name);
        Task<EmployeeDto?> SearchByNationalCodeAsync(string nationalCode);
        Task<EmployeeDto?> SearchByEmployeeCodeAsync(string employeeCode);
        #endregion

        #region Filter
        Task<List<EmployeeDto>> GetByStatusAsync(EmployeeStatus status);
        Task<List<EmployeeDto>> GetBySpecialityAsync(string speciality);
        #endregion
    }
}