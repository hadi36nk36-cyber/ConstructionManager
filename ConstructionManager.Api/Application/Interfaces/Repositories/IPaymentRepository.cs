using ConstructionManager.Api.Domain.Entities;
using ConstructionManager.Api.Domain.Enums;

namespace ConstructionManager.Api.Application.Interfaces.Repositories
{
    public interface IPaymentRepository
    {
        #region CRUD
        Task<List<Payment>> GetAllAsync();
        Task AddAsync(Payment payment);
        Task UpdateAsync(Payment payment);
        Task DeleteAsync(int id);
        #endregion

        #region Search
        Task<Payment?> SearchByCodeAsync(string code);
        Task<Payment?> SearchByTrackingCodeAsync(string trackingCode);
        #endregion

        #region Filter
        Task<List<Payment>> GetByTypeAsync(PaymentType type);
        Task<List<Payment>> GetByDateAsync(DateTime date);
        #endregion

        #region Validation
        Task<bool> IsCodeDuplicateAsync(string code);
        Task<bool> IsTrackingCodeDuplicateAsync(string trackingCode);
        #endregion
    }
}