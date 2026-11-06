using ConstructionManager.Api.Application.Interfaces.Repositories;
using ConstructionManager.Api.Domain.Entities;
using ConstructionManager.Api.Domain.Exceptions;
using ConstructionManager.Api.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace ConstructionManager.Api.Infrastructure.Repositories
{
    public class PurchaseInvoiceDetailRepository : IPurchaseInvoiceDetailRepository
    {
        private readonly AppDbContext _context;

        public PurchaseInvoiceDetailRepository(AppDbContext context)
        {
            _context = context;
        }

        #region CRUD
        public async Task<List<PurchaseInvoiceDetail>> GetAllAsync()
        {
            return await _context.PurchaseInvoiceDetails.ToListAsync();
        }

        public async Task AddAsync(PurchaseInvoiceDetail detail)
        {
            _context.PurchaseInvoiceDetails.Add(detail);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(PurchaseInvoiceDetail detail)
        {
            var existing = await _context.PurchaseInvoiceDetails.FindAsync(detail.Id);
            if (existing == null)
                throw new NotFoundException("PurchaseInvoiceDetail not found");

            _context.Entry(existing).CurrentValues.SetValues(detail);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var existing = await _context.PurchaseInvoiceDetails.FindAsync(id);
            if (existing == null)
                throw new NotFoundException("PurchaseInvoiceDetail not found");

            _context.PurchaseInvoiceDetails.Remove(existing);
            await _context.SaveChangesAsync();
        }
        #endregion
    }
}