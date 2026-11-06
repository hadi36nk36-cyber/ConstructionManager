using ConstructionManager.Api.Application.Interfaces.Repositories;
using ConstructionManager.Api.Domain.Entities;
using ConstructionManager.Api.Domain.Exceptions;
using ConstructionManager.Api.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace ConstructionManager.Api.Infrastructure.Repositories
{
    public class SupplierRepository : ISupplierRepository
    {
        private readonly AppDbContext _context;

        public SupplierRepository(AppDbContext context)
        {
            _context = context;
        }

        #region CRUD
        public async Task<List<Supplier>> GetAllAsync()
        {
            return await _context.Suppliers.ToListAsync();
        }

        public async Task AddAsync(Supplier supplier)
        {
            _context.Suppliers.Add(supplier);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(Supplier supplier)
        {
            var existing = await _context.Suppliers.FindAsync(supplier.Id);
            if (existing == null)
                throw new NotFoundException("Supplier not found");

            _context.Entry(existing).CurrentValues.SetValues(supplier);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var existing = await _context.Suppliers.FindAsync(id);
            if (existing == null)
                throw new NotFoundException("Supplier not found");

            _context.Suppliers.Remove(existing);
            await _context.SaveChangesAsync();
        }
        #endregion

        #region Search
        public async Task<Supplier?> SearchByCodeAsync(string code)
        {
            return await _context.Suppliers.FirstOrDefaultAsync(s => s.Code == code);
        }

        public async Task<List<Supplier>> SearchByNameAsync(string name)
        {
            return await _context.Suppliers.Where(s => s.Name.Contains(name)).ToListAsync();
        }

        public async Task<List<Supplier>> SearchByPhoneAsync(string phone)
        {
            return await _context.Suppliers.Where(s => s.Phone1.Contains(phone) || s.Phone2.Contains(phone)).ToListAsync();
        }
        #endregion

        #region Validation
        public async Task<bool> IsCodeDuplicateAsync(string code)
        {
            return await _context.Suppliers.AnyAsync(s => s.Code == code);
        }

        public async Task<bool> IsPhoneDuplicateAsync(string phone)
        {
            return await _context.Suppliers.AnyAsync(s => s.Phone1 == phone || s.Phone2 == phone);
        }
        #endregion

        #region Dependency
        public async Task<bool> HasPurchaseInvoicesAsync(int supplierId)
        {
            return await _context.PurchaseInvoices.AnyAsync(p => p.SupplierId == supplierId);
        }
        #endregion
    }
}