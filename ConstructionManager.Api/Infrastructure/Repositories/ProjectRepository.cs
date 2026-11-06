using ConstructionManager.Api.Application.Interfaces.Repositories;
using ConstructionManager.Api.Domain.Entities;
using ConstructionManager.Api.Domain.Enums;
using ConstructionManager.Api.Domain.Exceptions;
using ConstructionManager.Api.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace ConstructionManager.Api.Infrastructure.Repositories
{
    public class ProjectRepository : IProjectRepository
    {
        private readonly AppDbContext _context;

        public ProjectRepository(AppDbContext context)
        {
            _context = context;
        }

        #region CRUD
        public async Task<List<Project>> GetAllAsync()
        {
            return await _context.Projects.ToListAsync();
        }

        public async Task AddAsync(Project project)
        {
            _context.Projects.Add(project);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(Project project)
        {
            var existing = await _context.Projects.FindAsync(project.Id);
            if (existing == null)
                throw new NotFoundException("Project not found");

            _context.Entry(existing).CurrentValues.SetValues(project);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var existing = await _context.Projects.FindAsync(id);
            if (existing == null)
                throw new NotFoundException("Project not found");

            _context.Projects.Remove(existing);
            await _context.SaveChangesAsync();
        }
        #endregion

        #region Search
        public async Task<Project?> SearchByCodeAsync(string code)
        {
            return await _context.Projects.FirstOrDefaultAsync(p => p.Code == code);
        }

        public async Task<List<Project>> SearchByNameAsync(string name)
        {
            return await _context.Projects.Where(p => p.Name.Contains(name)).ToListAsync();
        }
        #endregion

        #region Filter
        public async Task<List<Project>> GetByStatusAsync(ProjectStatus status)
        {
            return await _context.Projects.Where(p => p.Status == status).ToListAsync();
        }

        public async Task<List<Project>> GetByCustomerAsync(int customerId)
        {
            return await _context.Projects.Where(p => p.CustomerId == customerId).ToListAsync();
        }

        public async Task<List<Project>> GetByDateRangeAsync(DateTime from, DateTime to)
        {
            return await _context.Projects.Where(p => p.StartDate >= from && p.EndDate <= to).ToListAsync();
        }
        #endregion

        #region Validation
        public async Task<bool> IsCodeDuplicateAsync(string code)
        {
            return await _context.Projects.AnyAsync(p => p.Code == code);
        }
        #endregion

        #region Dependency
        public async Task<bool> HasProjectAssignmentsAsync(int projectId)
        {
            return await _context.ProjectAssignments.AnyAsync(p => p.ProjectId == projectId);
        }

        public async Task<bool> HasInvoicesAsync(int projectId)
        {
            return await _context.Invoices.AnyAsync(i => i.ProjectId == projectId);
        }

        public async Task<bool> HasPurchaseInvoicesAsync(int projectId)
        {
            return await _context.PurchaseInvoices.AnyAsync(p => p.ProjectId == projectId);
        }
        #endregion
    }
}