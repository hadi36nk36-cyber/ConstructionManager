using ConstructionManager.Api.Application.DTOs;
using ConstructionManager.Api.Domain.Enums;

namespace ConstructionManager.Api.Application.Interfaces.Services
{
    public interface IPaymentService
    {
        #region CRUD
        Task<List<PaymentDto>> GetAllAsync();
        Task AddAsync(PaymentDto dto);
        Task UpdateAsync(PaymentDto dto);
        Task DeleteAsync(int id);
        #endregion

        #region Search
        Task<PaymentDto?> SearchByCodeAsync(string code);
        Task<PaymentDto?> SearchByTrackingCodeAsync(string trackingCode);
        #endregion

        #region Filter
        Task<List<PaymentDto>> GetByTypeAsync(PaymentType type);
        Task<List<PaymentDto>> GetByDateAsync(DateTime date);
        #endregion
    }
}