using ConstructionManager.Api.Application.DTOs;
using ConstructionManager.Api.Domain.Enums;

namespace ConstructionManager.Api.Application.Interfaces.Services
{
    public interface IProjectService
    {
        #region CRUD
        Task<List<ProjectDto>> GetAllAsync();
        Task AddAsync(ProjectDto dto);
        Task UpdateAsync(ProjectDto dto);
        Task DeleteAsync(int id);
        #endregion

        #region Search
        Task<ProjectDto?> SearchByCodeAsync(string code);
        Task<List<ProjectDto>> SearchByNameAsync(string name);
        #endregion

        #region Filter
        Task<List<ProjectDto>> GetByStatusAsync(ProjectStatus status);
        Task<List<ProjectDto>> GetByCustomerAsync(int customerId);
        Task<List<ProjectDto>> GetByDateRangeAsync(DateTime from, DateTime to);
        #endregion
    }
}