using ConstructionManager.Api.Domain.Entities;
using ConstructionManager.Api.Domain.Enums;

namespace ConstructionManager.Api.Application.Interfaces.Repositories
{
    public interface IProjectRepository
    {
        #region CRUD
        Task<List<Project>> GetAllAsync();
        Task AddAsync(Project project);
        Task UpdateAsync(Project project);
        Task DeleteAsync(int id);
        #endregion

        #region Search
        Task<Project?> SearchByCodeAsync(string code);
        Task<List<Project>> SearchByNameAsync(string name);
        #endregion

        #region Filter
        Task<List<Project>> GetByStatusAsync(ProjectStatus status);
        Task<List<Project>> GetByCustomerAsync(int customerId);
        Task<List<Project>> GetByDateRangeAsync(DateTime from, DateTime to);
        #endregion

        #region Validation
        Task<bool> IsCodeDuplicateAsync(string code);
        #endregion

        #region Dependency
        Task<bool> HasProjectAssignmentsAsync(int projectId);
        Task<bool> HasInvoicesAsync(int projectId);
        Task<bool> HasPurchaseInvoicesAsync(int projectId);
        #endregion
    }
}