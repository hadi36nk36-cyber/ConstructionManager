using ConstructionManager.Api.Domain.Entities;
using ConstructionManager.Api.Domain.Enums;

namespace ConstructionManager.Api.Application.Interfaces.Repositories
{
    public interface IEquipmentRepository
    {
        #region CRUD
        Task<List<Equipment>> GetAllAsync();
        Task AddAsync(Equipment equipment);
        Task UpdateAsync(Equipment equipment);
        Task DeleteAsync(int id);
        #endregion

        #region Search
        Task<Equipment?> SearchByCodeAsync(string code);
        Task<Equipment?> SearchBySerialNumberAsync(string serialNumber);
        #endregion

        #region Filter
        Task<List<Equipment>> GetByTypeAsync(EquipmentType type);
        Task<List<Equipment>> GetByStatusAsync(EquipmentStatus status);
        #endregion

        #region Validation
        Task<bool> IsCodeDuplicateAsync(string code);
        Task<bool> IsSerialNumberDuplicateAsync(string serialNumber);
        #endregion

        #region Dependency
        Task<bool> HasPaymentsAsync(int equipmentId);
        #endregion
    }
}