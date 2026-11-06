using ConstructionManager.Api.Application.Interfaces.Repositories;
using ConstructionManager.Api.Domain.Entities;
using ConstructionManager.Api.Domain.Exceptions;
using ConstructionManager.Api.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace ConstructionManager.Api.Infrastructure.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly AppDbContext _context;

        public UserRepository(AppDbContext context)
        {
            _context = context;
        }

        #region CRUD
        public async Task<List<User>> GetAllAsync()
        {
            return await _context.Users.Where(u => u.IsActive).ToListAsync();
        }

        public async Task AddAsync(User user)
        {
            _context.Users.Add(user);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(User user)
        {
            var existing = await _context.Users.FindAsync(user.Id);
            if (existing == null)
                throw new NotFoundException("User not found");

            _context.Entry(existing).CurrentValues.SetValues(user);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var existing = await _context.Users.FindAsync(id);
            if (existing == null)
                throw new NotFoundException("User not found");

            existing.IsActive = false;   
            await _context.SaveChangesAsync();
        }
        #endregion

        #region Search
        public async Task<User?> SearchByUsernameAsync(string username)
        {
            return await _context.Users.FirstOrDefaultAsync(u => u.Username == username);
        }

        public async Task<User?> GetByIdAsync(int id)
        {
            return await _context.Users.FindAsync(id);
        }
        #endregion

        #region Filter
        public async Task<List<User>> GetByRoleAsync(string role)
        {
            return await _context.Users.Where(u => u.Role == role && u.IsActive).ToListAsync();
        }
        #endregion

        #region Validation
        public async Task<bool> IsUsernameDuplicateAsync(string username)
        {
            return await _context.Users.AnyAsync(u => u.Username == username);
        }
        #endregion
        public async Task<User?> GetByRefreshTokenAsync(string refreshToken)
        {
            return await _context.Users.FirstOrDefaultAsync(u => u.RefreshToken == refreshToken);
        }

    }
}