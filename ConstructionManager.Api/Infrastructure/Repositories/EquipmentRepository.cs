using ConstructionManager.Api.Application.Interfaces.Repositories;
using ConstructionManager.Api.Domain.Entities;
using ConstructionManager.Api.Domain.Enums;
using ConstructionManager.Api.Domain.Exceptions;
using ConstructionManager.Api.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace ConstructionManager.Api.Infrastructure.Repositories
{
    public class EquipmentRepository : IEquipmentRepository
    {
        private readonly AppDbContext _context;

        public EquipmentRepository(AppDbContext context)
        {
            _context = context;
        }

        #region CRUD
        public async Task<List<Equipment>> GetAllAsync()
        {
            return await _context.Equipments.ToListAsync();
        }

        public async Task AddAsync(Equipment equipment)
        {
            _context.Equipments.Add(equipment);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(Equipment equipment)
        {
            var existing = await _context.Equipments.FindAsync(equipment.Id);
            if (existing == null)
                throw new NotFoundException("Equipment not found");

            _context.Entry(existing).CurrentValues.SetValues(equipment);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var existing = await _context.Equipments.FindAsync(id);
            if (existing == null)
                throw new NotFoundException("Equipment not found");

            _context.Equipments.Remove(existing);
            await _context.SaveChangesAsync();
        }
        #endregion

        #region Search
        public async Task<Equipment?> SearchByCodeAsync(string code)
        {
            return await _context.Equipments.FirstOrDefaultAsync(e => e.Code == code);
        }

        public async Task<Equipment?> SearchBySerialNumberAsync(string serialNumber)
        {
            return await _context.Equipments.FirstOrDefaultAsync(e => e.SerialNumber == serialNumber);
        }
        #endregion

        #region Filter
        public async Task<List<Equipment>> GetByTypeAsync(EquipmentType type)
        {
            return await _context.Equipments.Where(e => e.Type == type).ToListAsync();
        }

        public async Task<List<Equipment>> GetByStatusAsync(EquipmentStatus status)
        {
            return await _context.Equipments.Where(e => e.Status == status).ToListAsync();
        }
        #endregion

        #region Validation
        public async Task<bool> IsCodeDuplicateAsync(string code)
        {
            return await _context.Equipments.AnyAsync(e => e.Code == code);
        }

        public async Task<bool> IsSerialNumberDuplicateAsync(string serialNumber)
        {
            return await _context.Equipments.AnyAsync(e => e.SerialNumber == serialNumber);
        }
        #endregion

        #region Dependency
        public async Task<bool> HasPaymentsAsync(int equipmentId)
        {
            return await _context.Payments.AnyAsync(p => p.EquipmentId == equipmentId);
        }
        #endregion
    }
}