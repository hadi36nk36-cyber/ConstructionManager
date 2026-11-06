using ConstructionManager.Api.Domain.Entities;

namespace ConstructionManager.Api.Application.Interfaces.Repositories
{
    public interface IPurchaseInvoiceDetailRepository
    {
        #region CRUD
        Task<List<PurchaseInvoiceDetail>> GetAllAsync();
        Task AddAsync(PurchaseInvoiceDetail detail);
        Task UpdateAsync(PurchaseInvoiceDetail detail);
        Task DeleteAsync(int id);
        #endregion
    }
}