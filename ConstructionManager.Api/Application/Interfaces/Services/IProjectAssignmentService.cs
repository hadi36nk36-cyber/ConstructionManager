using ConstructionManager.Api.Application.DTOs;

namespace ConstructionManager.Api.Application.Interfaces.Services
{
    public interface IProjectAssignmentService
    {
        #region CRUD
        Task<List<ProjectAssignmentDto>> GetAllAsync();
        Task AddAsync(ProjectAssignmentDto dto);
        Task UpdateAsync(ProjectAssignmentDto dto);
        Task DeleteAsync(int id);
        #endregion

        #region Search
        Task<List<ProjectAssignmentDto>> SearchByEmployeeAsync(int employeeId);
        Task<List<ProjectAssignmentDto>> SearchByProjectAsync(int projectId);
        #endregion

        #region Filter
        Task<List<ProjectAssignmentDto>> GetByRoleAsync(string role);
        Task<List<ProjectAssignmentDto>> GetByDateAsync(DateTime date);
        #endregion
    }
}