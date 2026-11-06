using ConstructionManager.Api.Application.Interfaces.Repositories;
using ConstructionManager.Api.Domain.Entities;
using ConstructionManager.Api.Domain.Exceptions;
using ConstructionManager.Api.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace ConstructionManager.Api.Infrastructure.Repositories
{
    public class ProjectAssignmentRepository : IProjectAssignmentRepository
    {
        private readonly AppDbContext _context;

        public ProjectAssignmentRepository(AppDbContext context)
        {
            _context = context;
        }

        #region CRUD
        public async Task<List<ProjectAssignment>> GetAllAsync()
        {
            return await _context.ProjectAssignments.ToListAsync();
        }

        public async Task AddAsync(ProjectAssignment assignment)
        {
            _context.ProjectAssignments.Add(assignment);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(ProjectAssignment assignment)
        {
            var existing = await _context.ProjectAssignments.FindAsync(assignment.Id);
            if (existing == null)
                throw new NotFoundException("ProjectAssignment not found");

            _context.Entry(existing).CurrentValues.SetValues(assignment);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var existing = await _context.ProjectAssignments.FindAsync(id);
            if (existing == null)
                throw new NotFoundException("ProjectAssignment not found");

            _context.ProjectAssignments.Remove(existing);
            await _context.SaveChangesAsync();
        }
        #endregion

        #region Search
        public async Task<List<ProjectAssignment>> SearchByEmployeeAsync(int employeeId)
        {
            return await _context.ProjectAssignments.Where(p => p.EmployeeId == employeeId).ToListAsync();
        }

        public async Task<List<ProjectAssignment>> SearchByProjectAsync(int projectId)
        {
            return await _context.ProjectAssignments.Where(p => p.ProjectId == projectId).ToListAsync();
        }
        #endregion

        #region Filter
        public async Task<List<ProjectAssignment>> GetByRoleAsync(string role)
        {
            return await _context.ProjectAssignments.Where(p => p.Role.Contains(role)).ToListAsync();
        }

        public async Task<List<ProjectAssignment>> GetByDateAsync(DateTime date)
        {
            return await _context.ProjectAssignments.Where(p => p.JoinDate.Date == date.Date).ToListAsync();
        }
        #endregion

        #region Validation
        public async Task<bool> IsDuplicateAsync(int projectId, int employeeId)
        {
            return await _context.ProjectAssignments.AnyAsync(p => p.ProjectId == projectId && p.EmployeeId == employeeId);
        }
        #endregion
    }
}