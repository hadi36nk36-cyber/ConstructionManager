using ConstructionManager.Api.Application.Interfaces.Repositories;
using ConstructionManager.Api.Domain.Entities;
using ConstructionManager.Api.Domain.Exceptions;
using ConstructionManager.Api.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace ConstructionManager.Api.Infrastructure.Repositories
{
    public class MaterialRepository : IMaterialRepository
    {
        private readonly AppDbContext _context;

        public MaterialRepository(AppDbContext context)
        {
            _context = context;
        }

        #region CRUD
        public async Task<List<Material>> GetAllAsync()
        {
            return await _context.Materials.ToListAsync();
        }

        public async Task AddAsync(Material material)
        {
            _context.Materials.Add(material);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(Material material)
        {
            var existing = await _context.Materials.FindAsync(material.Id);
            if (existing == null)
                throw new NotFoundException("Material not found");

            _context.Entry(existing).CurrentValues.SetValues(material);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var existing = await _context.Materials.FindAsync(id);
            if (existing == null)
                throw new NotFoundException("Material not found");

            _context.Materials.Remove(existing);
            await _context.SaveChangesAsync();
        }
        #endregion

        #region Search
        public async Task<Material?> SearchByCodeAsync(string code)
        {
            return await _context.Materials.FirstOrDefaultAsync(m => m.Code == code);
        }

        public async Task<List<Material>> SearchByNameAsync(string name)
        {
            return await _context.Materials.Where(m => m.Name.Contains(name)).ToListAsync();
        }
        #endregion

        #region Filter
        public async Task<List<Material>> GetByCategoryAsync(string category)
        {
            return await _context.Materials.Where(m => m.Category.Contains(category)).ToListAsync();
        }
        #endregion

        #region Validation
        public async Task<bool> IsCodeDuplicateAsync(string code)
        {
            return await _context.Materials.AnyAsync(m => m.Code == code);
        }
        #endregion
    }
}