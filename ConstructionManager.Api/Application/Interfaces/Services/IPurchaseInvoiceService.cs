using ConstructionManager.Api.Application.DTOs;
using ConstructionManager.Api.Domain.Enums;

namespace ConstructionManager.Api.Application.Interfaces.Services
{
    public interface IPurchaseInvoiceService
    {
        #region CRUD
        Task<List<PurchaseInvoiceDto>> GetAllAsync();
        Task AddAsync(PurchaseInvoiceDto dto);
        Task UpdateAsync(PurchaseInvoiceDto dto);
        Task DeleteAsync(int id);
        #endregion

        #region Search
        Task<PurchaseInvoiceDto?> SearchByCodeAsync(string code);
        Task<PurchaseInvoiceDto?> SearchByTrackingCodeAsync(string trackingCode);
        #endregion

        #region Filter
        Task<List<PurchaseInvoiceDto>> GetByInvoiceDateAsync(DateTime date);
        #endregion
    }
}