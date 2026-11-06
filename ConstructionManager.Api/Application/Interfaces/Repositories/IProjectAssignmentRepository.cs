using ConstructionManager.Api.Domain.Entities;

namespace ConstructionManager.Api.Application.Interfaces.Repositories
{
    public interface IProjectAssignmentRepository
    {
        #region CRUD
        Task<List<ProjectAssignment>> GetAllAsync();
        Task AddAsync(ProjectAssignment assignment);
        Task UpdateAsync(ProjectAssignment assignment);
        Task DeleteAsync(int id);
        #endregion

        #region Search
        Task<List<ProjectAssignment>> SearchByEmployeeAsync(int employeeId);
        Task<List<ProjectAssignment>> SearchByProjectAsync(int projectId);
        #endregion

        #region Filter
        Task<List<ProjectAssignment>> GetByRoleAsync(string role);
        Task<List<ProjectAssignment>> GetByDateAsync(DateTime date);
        #endregion

        #region Validation
        Task<bool> IsDuplicateAsync(int projectId, int employeeId);
        #endregion
    }
}