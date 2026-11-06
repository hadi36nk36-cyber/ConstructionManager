using ConstructionManager.Api.Application.Interfaces.Repositories;
using ConstructionManager.Api.Domain.Entities;
using ConstructionManager.Api.Domain.Enums;
using ConstructionManager.Api.Domain.Exceptions;
using ConstructionManager.Api.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace ConstructionManager.Api.Infrastructure.Repositories
{
    public class CustomerRepository : ICustomerRepository
    {
        private readonly AppDbContext _context;

        public CustomerRepository(AppDbContext context)
        {
            _context = context;
        }

        #region CRUD
        public async Task<List<Customer>> GetAllAsync()
        {
            return await _context.Customers.ToListAsync();
        }

        public async Task AddAsync(Customer customer)
        {
            _context.Customers.Add(customer);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(Customer customer)
        {
            var existing = await _context.Customers.FindAsync(customer.Id);
            if (existing == null)
                throw new NotFoundException("Customer not found");

            _context.Entry(existing).CurrentValues.SetValues(customer);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var existing = await _context.Customers.FindAsync(id);
            if (existing == null)
                throw new NotFoundException("Customer not found");

            _context.Customers.Remove(existing);
            await _context.SaveChangesAsync();
        }
        #endregion

        #region Search
        public async Task<Customer?> SearchByCodeAsync(string code)
        {
            return await _context.Customers.FirstOrDefaultAsync(c => c.Code == code);
        }

        public async Task<List<Customer>> SearchByNameAsync(string name)
        {
            return await _context.Customers.Where(c => c.Name.Contains(name)).ToListAsync();
        }

        public async Task<List<Customer>> SearchByPhoneAsync(string phone)
        {
            return await _context.Customers.Where(c => c.Phone1.Contains(phone) || c.Phone2.Contains(phone)).ToListAsync();
        }
        #endregion

        #region Filter
        public async Task<List<Customer>> GetByTypeAsync(CustomerType type)
        {
            return await _context.Customers.Where(c => c.CustomerType == type).ToListAsync();
        }
        #endregion

        #region Validation
        public async Task<bool> IsCodeDuplicateAsync(string code)
        {
            return await _context.Customers.AnyAsync(c => c.Code == code);
        }

        public async Task<bool> IsPhoneDuplicateAsync(string phone)
        {
            return await _context.Customers.AnyAsync(c => c.Phone1 == phone || c.Phone2 == phone);
        }
        #endregion

        #region Dependency
        public async Task<bool> HasProjectsAsync(int customerId)
        {
            return await _context.Projects.AnyAsync(p => p.CustomerId == customerId);
        }

        public async Task<bool> HasInvoicesAsync(int customerId)
        {
            return await _context.Invoices.AnyAsync(i => i.CustomerId == customerId);
        }
        #endregion
    }
}