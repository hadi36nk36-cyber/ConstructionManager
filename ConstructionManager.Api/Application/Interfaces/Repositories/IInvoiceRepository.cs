using ConstructionManager.Api.Domain.Entities;
using ConstructionManager.Api.Domain.Enums;

namespace ConstructionManager.Api.Application.Interfaces.Repositories
{
    public interface IInvoiceRepository
    {
        #region CRUD
        Task<List<Invoice>> GetAllAsync();
        Task AddAsync(Invoice invoice);
        Task UpdateAsync(Invoice invoice);
        Task DeleteAsync(int id);
        #endregion

        #region Search
        Task<Invoice?> SearchByCodeAsync(string code);
        Task<Invoice?> SearchByTrackingCodeAsync(string trackingCode);
        #endregion

        #region Filter
        Task<List<Invoice>> GetByStatusAsync(InvoiceStatus status);
        Task<List<Invoice>> GetByDateRangeAsync(DateTime from, DateTime to);
        #endregion

        #region Validation
        Task<bool> IsCodeDuplicateAsync(string code);
        Task<bool> IsTrackingCodeDuplicateAsync(string trackingCode);
        Task<bool> IsProjectIdDuplicateAsync(int projectId);
        #endregion

        #region Dependency
        Task<bool> HasPurchaseInvoicesAsync(int invoiceId);
        #endregion
    }
}