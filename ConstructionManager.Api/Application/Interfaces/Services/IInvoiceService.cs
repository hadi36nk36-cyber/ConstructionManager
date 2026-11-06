using ConstructionManager.Api.Application.DTOs;
using ConstructionManager.Api.Domain.Enums;

namespace ConstructionManager.Api.Application.Interfaces.Services
{
    public interface IInvoiceService
    {
        #region CRUD
        Task<List<InvoiceDto>> GetAllAsync();
        Task AddAsync(InvoiceDto dto);
        Task UpdateAsync(InvoiceDto dto);
        Task DeleteAsync(int id);
        #endregion

        #region Search
        Task<InvoiceDto?> SearchByCodeAsync(string code);
        Task<InvoiceDto?> SearchByTrackingCodeAsync(string trackingCode);
        #endregion

        #region Filter
        Task<List<InvoiceDto>> GetByStatusAsync(InvoiceStatus status);
        Task<List<InvoiceDto>> GetByDateRangeAsync(DateTime from, DateTime to);
        #endregion
    }
}