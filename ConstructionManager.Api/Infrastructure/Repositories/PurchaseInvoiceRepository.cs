using ConstructionManager.Api.Application.Interfaces.Repositories;
using ConstructionManager.Api.Domain.Entities;
using ConstructionManager.Api.Domain.Exceptions;
using ConstructionManager.Api.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace ConstructionManager.Api.Infrastructure.Repositories
{
    public class PurchaseInvoiceRepository : IPurchaseInvoiceRepository
    {
        private readonly AppDbContext _context;

        public PurchaseInvoiceRepository(AppDbContext context)
        {
            _context = context;
        }

        #region CRUD
        public async Task<List<PurchaseInvoice>> GetAllAsync()
        {
            return await _context.PurchaseInvoices.ToListAsync();
        }

        public async Task AddAsync(PurchaseInvoice purchaseInvoice)
        {
            _context.PurchaseInvoices.Add(purchaseInvoice);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(PurchaseInvoice purchaseInvoice)
        {
            var existing = await _context.PurchaseInvoices.FindAsync(purchaseInvoice.Id);
            if (existing == null)
                throw new NotFoundException("PurchaseInvoice not found");

            _context.Entry(existing).CurrentValues.SetValues(purchaseInvoice);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var existing = await _context.PurchaseInvoices.FindAsync(id);
            if (existing == null)
                throw new NotFoundException("PurchaseInvoice not found");

            _context.PurchaseInvoices.Remove(existing);
            await _context.SaveChangesAsync();
        }
        #endregion

        #region Search
        public async Task<PurchaseInvoice?> SearchByCodeAsync(string code)
        {
            return await _context.PurchaseInvoices.FirstOrDefaultAsync(p => p.Code == code);
        }

        public async Task<PurchaseInvoice?> SearchByTrackingCodeAsync(string trackingCode)
        {
            return await _context.PurchaseInvoices.FirstOrDefaultAsync(p => p.TrackingCode == trackingCode);
        }
        #endregion

        #region Filter
        public async Task<List<PurchaseInvoice>> GetByInvoiceDateAsync(DateTime date)
        {
            return await _context.PurchaseInvoices.Where(p => p.InvoiceDate.Date == date.Date).ToListAsync();
        }
        #endregion

        #region Validation
        public async Task<bool> IsCodeDuplicateAsync(string code)
        {
            return await _context.PurchaseInvoices.AnyAsync(p => p.Code == code);
        }

        public async Task<bool> IsTrackingCodeDuplicateAsync(string trackingCode)
        {
            return await _context.PurchaseInvoices.AnyAsync(p => p.TrackingCode == trackingCode);
        }
        #endregion

        #region Dependency
        public async Task<bool> HasDetailsAsync(int purchaseInvoiceId)
        {
            return await _context.PurchaseInvoiceDetails.AnyAsync(d => d.PurchaseInvoiceId == purchaseInvoiceId);
        }

        public async Task<bool> HasPaymentsAsync(int purchaseInvoiceId)
        {
            return await _context.Payments.AnyAsync(p => p.PurchaseInvoiceId == purchaseInvoiceId);
        }
        #endregion
    }
}