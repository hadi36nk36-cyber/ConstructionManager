using ConstructionManager.Api.Application.Interfaces.Repositories;
using ConstructionManager.Api.Domain.Entities;
using ConstructionManager.Api.Domain.Enums;
using ConstructionManager.Api.Domain.Exceptions;
using ConstructionManager.Api.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace ConstructionManager.Api.Infrastructure.Repositories
{
    public class PaymentRepository : IPaymentRepository
    {
        private readonly AppDbContext _context;

        public PaymentRepository(AppDbContext context)
        {
            _context = context;
        }

        #region CRUD
        public async Task<List<Payment>> GetAllAsync()
        {
            return await _context.Payments.ToListAsync();
        }

        public async Task AddAsync(Payment payment)
        {
            _context.Payments.Add(payment);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(Payment payment)
        {
            var existing = await _context.Payments.FindAsync(payment.Id);
            if (existing == null)
                throw new NotFoundException("Payment not found");

            _context.Entry(existing).CurrentValues.SetValues(payment);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var existing = await _context.Payments.FindAsync(id);
            if (existing == null)
                throw new NotFoundException("Payment not found");

            _context.Payments.Remove(existing);
            await _context.SaveChangesAsync();
        }
        #endregion

        #region Search
        public async Task<Payment?> SearchByCodeAsync(string code)
        {
            return await _context.Payments.FirstOrDefaultAsync(p => p.Code == code);
        }

        public async Task<Payment?> SearchByTrackingCodeAsync(string trackingCode)
        {
            return await _context.Payments.FirstOrDefaultAsync(p => p.TrackingCode == trackingCode);
        }
        #endregion

        #region Filter
        public async Task<List<Payment>> GetByTypeAsync(PaymentType type)
        {
            return await _context.Payments.Where(p => p.PaymentType == type).ToListAsync();
        }

        public async Task<List<Payment>> GetByDateAsync(DateTime date)
        {
            return await _context.Payments.Where(p => p.PaymentDate.Date == date.Date).ToListAsync();
        }
        #endregion

        #region Validation
        public async Task<bool> IsCodeDuplicateAsync(string code)
        {
            return await _context.Payments.AnyAsync(p => p.Code == code);
        }

        public async Task<bool> IsTrackingCodeDuplicateAsync(string trackingCode)
        {
            return await _context.Payments.AnyAsync(p => p.TrackingCode == trackingCode);
        }
        #endregion
    }
}