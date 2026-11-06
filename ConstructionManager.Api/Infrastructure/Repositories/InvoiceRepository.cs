using ConstructionManager.Api.Application.Interfaces.Repositories;
using ConstructionManager.Api.Domain.Entities;
using ConstructionManager.Api.Domain.Enums;
using ConstructionManager.Api.Domain.Exceptions;
using ConstructionManager.Api.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace ConstructionManager.Api.Infrastructure.Repositories
{
    public class InvoiceRepository : IInvoiceRepository
    {
        private readonly AppDbContext _context;

        public InvoiceRepository(AppDbContext context)
        {
            _context = context;
        }

        #region CRUD
        public async Task<List<Invoice>> GetAllAsync()
        {
            return await _context.Invoices.ToListAsync();
        }

        public async Task AddAsync(Invoice invoice)
        {
            _context.Invoices.Add(invoice);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(Invoice invoice)
        {
            var existing = await _context.Invoices.FindAsync(invoice.Id);
            if (existing == null)
                throw new NotFoundException("Invoice not found");

            _context.Entry(existing).CurrentValues.SetValues(invoice);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var existing = await _context.Invoices.FindAsync(id);
            if (existing == null)
                throw new NotFoundException("Invoice not found");

            _context.Invoices.Remove(existing);
            await _context.SaveChangesAsync();
        }
        #endregion

        #region Search
        public async Task<Invoice?> SearchByCodeAsync(string code)
        {
            return await _context.Invoices.FirstOrDefaultAsync(i => i.Code == code);
        }

        public async Task<Invoice?> SearchByTrackingCodeAsync(string trackingCode)
        {
            return await _context.Invoices.FirstOrDefaultAsync(i => i.TrackingCode == trackingCode);
        }
        #endregion

        #region Filter
        public async Task<List<Invoice>> GetByStatusAsync(InvoiceStatus status)
        {
            return await _context.Invoices.Where(i => i.Status == status).ToListAsync();
        }

        public async Task<List<Invoice>> GetByDateRangeAsync(DateTime from, DateTime to)
        {
            return await _context.Invoices.Where(i => i.InvoiceDate >= from && i.InvoiceDate <= to).ToListAsync();
        }
        #endregion

        #region Validation
        public async Task<bool> IsCodeDuplicateAsync(string code)
        {
            return await _context.Invoices.AnyAsync(i => i.Code == code);
        }

        public async Task<bool> IsTrackingCodeDuplicateAsync(string trackingCode)
        {
            return await _context.Invoices.AnyAsync(i => i.TrackingCode == trackingCode);
        }

        public async Task<bool> IsProjectIdDuplicateAsync(int projectId)
        {
            return await _context.Invoices.AnyAsync(i => i.ProjectId == projectId);
        }
        #endregion

        #region Dependency
        public async Task<bool> HasPurchaseInvoicesAsync(int invoiceId)
        {
            return await _context.PurchaseInvoices.AnyAsync(p => p.InvoiceId == invoiceId);
        }
        #endregion
    }
}