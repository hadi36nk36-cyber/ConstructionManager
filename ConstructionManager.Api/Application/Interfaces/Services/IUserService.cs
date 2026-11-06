using ConstructionManager.Api.Application.DTOs;

namespace ConstructionManager.Api.Application.Interfaces.Services
{
    public interface IUserService
    {
        #region CRUD
        Task<List<UserDto>> GetAllAsync();
        Task AddAsync(UserDto dto);
        Task UpdateAsync(UserDto dto);
        Task DeleteAsync(int id);
        #endregion

        #region Search
        Task<UserDto?> SearchByUsernameAsync(string username);
        #endregion

        #region Filter
        Task<List<UserDto>> GetByRoleAsync(string role);
        #endregion
    }
}