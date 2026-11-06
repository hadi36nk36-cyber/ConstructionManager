using ConstructionManager.Api.Domain.Entities;

namespace ConstructionManager.Api.Application.Interfaces.Repositories
{
    public interface IUserRepository
    {
        #region CRUD
        Task<List<User>> GetAllAsync();
        Task AddAsync(User user);
        Task UpdateAsync(User user);
        Task DeleteAsync(int id);
        #endregion

        #region Search
        Task<User?> SearchByUsernameAsync(string username);
        Task<User?> GetByIdAsync(int id);
        #endregion

        #region Filter
        Task<List<User>> GetByRoleAsync(string role);
        #endregion

        #region Validation
        Task<bool> IsUsernameDuplicateAsync(string username);
        #endregion
        Task<User?> GetByRefreshTokenAsync(string refreshToken);
    }
}