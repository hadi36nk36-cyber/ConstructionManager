using ConstructionManager.Api.Application.DTOs;
using ConstructionManager.Api.Domain.Enums;

namespace ConstructionManager.Api.Application.Interfaces.Services
{
    public interface IEquipmentService
    {
        #region CRUD
        Task<List<EquipmentDto>> GetAllAsync();
        Task AddAsync(EquipmentDto dto);
        Task UpdateAsync(EquipmentDto dto);
        Task DeleteAsync(int id);
        #endregion

        #region Search
        Task<EquipmentDto?> SearchByCodeAsync(string code);
        Task<EquipmentDto?> SearchBySerialNumberAsync(string serialNumber);
        #endregion

        #region Filter
        Task<List<EquipmentDto>> GetByTypeAsync(EquipmentType type);
        Task<List<EquipmentDto>> GetByStatusAsync(EquipmentStatus status);
        #endregion
    }
}