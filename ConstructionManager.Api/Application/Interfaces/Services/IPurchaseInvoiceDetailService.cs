using ConstructionManager.Api.Application.DTOs;

namespace ConstructionManager.Api.Application.Interfaces.Services
{
    public interface IPurchaseInvoiceDetailService
    {
        #region CRUD
        Task<List<PurchaseInvoiceDetailDto>> GetAllAsync();
        Task AddAsync(PurchaseInvoiceDetailDto dto);
        Task UpdateAsync(PurchaseInvoiceDetailDto dto);
        Task DeleteAsync(int id);
        #endregion
    }
}