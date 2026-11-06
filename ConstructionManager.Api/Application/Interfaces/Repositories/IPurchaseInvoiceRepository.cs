using ConstructionManager.Api.Domain.Entities;

namespace ConstructionManager.Api.Application.Interfaces.Repositories
{
    public interface IPurchaseInvoiceRepository
    {
        #region CRUD
        Task<List<PurchaseInvoice>> GetAllAsync();
        Task AddAsync(PurchaseInvoice purchaseInvoice);
        Task UpdateAsync(PurchaseInvoice purchaseInvoice);
        Task DeleteAsync(int id);
        #endregion

        #region Search
        Task<PurchaseInvoice?> SearchByCodeAsync(string code);
        Task<PurchaseInvoice?> SearchByTrackingCodeAsync(string trackingCode);
        #endregion

        #region Filter
        Task<List<PurchaseInvoice>> GetByInvoiceDateAsync(DateTime date);
        #endregion

        #region Validation
        Task<bool> IsCodeDuplicateAsync(string code);
        Task<bool> IsTrackingCodeDuplicateAsync(string trackingCode);
        #endregion

        #region Dependency
        Task<bool> HasDetailsAsync(int purchaseInvoiceId);
        Task<bool> HasPaymentsAsync(int purchaseInvoiceId);
        #endregion
    }
}