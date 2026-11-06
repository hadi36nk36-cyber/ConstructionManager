using ConstructionManager.Api.Application.Interfaces.Repositories;
using ConstructionManager.Api.Domain.Entities;
using ConstructionManager.Api.Domain.Enums;
using ConstructionManager.Api.Domain.Exceptions;
using ConstructionManager.Api.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace ConstructionManager.Api.Infrastructure.Repositories
{
    public class EmployeeRepository : IEmployeeRepository
    {
        private readonly AppDbContext _context;

        public EmployeeRepository(AppDbContext context)
        {
            _context = context;
        }

        #region CRUD
        public async Task<List<Employee>> GetAllAsync()
        {
            return await _context.Employees.ToListAsync();
        }

        public async Task AddAsync(Employee employee)
        {
            _context.Employees.Add(employee);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(Employee employee)
        {
            var existing = await _context.Employees.FindAsync(employee.Id);
            if (existing == null)
                throw new NotFoundException("Employee not found");

            _context.Entry(existing).CurrentValues.SetValues(employee);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var existing = await _context.Employees.FindAsync(id);
            if (existing == null)
                throw new NotFoundException("Employee not found");

            _context.Employees.Remove(existing);
            await _context.SaveChangesAsync();
        }
        #endregion

        #region Search
        public async Task<List<Employee>> SearchByNameAsync(string name)
        {
            return await _context.Employees.Where(e => e.FirstName.Contains(name) || e.LastName.Contains(name)).ToListAsync();
        }

        public async Task<Employee?> SearchByNationalCodeAsync(string nationalCode)
        {
            return await _context.Employees.FirstOrDefaultAsync(e => e.NationalCode == nationalCode);
        }

        public async Task<Employee?> SearchByEmployeeCodeAsync(string employeeCode)
        {
            return await _context.Employees.FirstOrDefaultAsync(e => e.EmployeeCode == employeeCode);
        }
        #endregion

        #region Filter
        public async Task<List<Employee>> GetByStatusAsync(EmployeeStatus status)
        {
            return await _context.Employees.Where(e => e.Status == status).ToListAsync();
        }

        public async Task<List<Employee>> GetBySpecialityAsync(string speciality)
        {
            return await _context.Employees.Where(e => e.Speciality.Contains(speciality)).ToListAsync();
        }
        #endregion

        #region Validation
        public async Task<bool> IsNationalCodeDuplicateAsync(string nationalCode)
        {
            return await _context.Employees.AnyAsync(e => e.NationalCode == nationalCode);
        }

        public async Task<bool> IsEmployeeCodeDuplicateAsync(string employeeCode)
        {
            return await _context.Employees.AnyAsync(e => e.EmployeeCode == employeeCode);
        }

        public async Task<bool> IsPhoneDuplicateAsync(string phone)
        {
            return await _context.Employees.AnyAsync(e => e.Phone1 == phone || e.Phone2 == phone);
        }
        #endregion

        #region Dependency
        public async Task<bool> HasProjectAssignmentsAsync(int employeeId)
        {
            return await _context.ProjectAssignments.AnyAsync(p => p.EmployeeId == employeeId);
        }

        public async Task<bool> HasPaymentsAsync(int employeeId)
        {
            return await _context.Payments.AnyAsync(p => p.EmployeeId == employeeId);
        }
        #endregion
    }
}