using AutoMapper;
using ConstructionManager.Api.Application.DTOs;
using ConstructionManager.Api.Application.Interfaces.Repositories;
using ConstructionManager.Api.Application.Interfaces.Services;
using ConstructionManager.Api.Domain.Entities;
using ConstructionManager.Api.Domain.Exceptions;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace ConstructionManager.Api.Application.Services
{
    public class ProjectAssignmentService : IProjectAssignmentService
    {
        private readonly IProjectAssignmentRepository _repository;
        private readonly IMapper _mapper;
        private readonly ILogger<ProjectAssignmentService> _logger;

        public ProjectAssignmentService(IProjectAssignmentRepository repository, IMapper mapper, ILogger<ProjectAssignmentService> logger)
        {
            _repository = repository;
            _mapper = mapper;
            _logger = logger;
        }

        #region CRUD
        public async Task<List<ProjectAssignmentDto>> GetAllAsync()
        {
            try
            {
                var assignments = await _repository.GetAllAsync();
                return _mapper.Map<List<ProjectAssignmentDto>>(assignments);
            }
            catch (SqlException ex)
            {
                _logger.LogError(ex, "Database connection error in {Operation} ProjectAssignment", nameof(GetAllAsync));
                throw new BusinessRuleException("Database is unavailable");
            }
        }

        public async Task AddAsync(ProjectAssignmentDto dto)
        {
            try
            {
                if (await _repository.IsDuplicateAsync(dto.ProjectId, dto.EmployeeId))
                    throw new DuplicateException("Employee is already assigned to this project");

                if (dto.EndDate.HasValue && dto.EndDate < dto.JoinDate)
                    throw new ValidationException("End date cannot be before join date");

                var assignment = _mapper.Map<ProjectAssignment>(dto);
                await _repository.AddAsync(assignment);
            }
            catch (DuplicateException ex)
            {
                _logger.LogWarning(ex, "Duplicate project assignment in {Operation}: ProjectId={ProjectId}, EmployeeId={EmployeeId}",
                    nameof(AddAsync), dto.ProjectId, dto.EmployeeId);
                throw;
            }
            catch (ValidationException ex)
            {
                _logger.LogWarning(ex, "Validation error in {Operation} ProjectAssignment: ProjectId={ProjectId}", nameof(AddAsync), dto.ProjectId);
                throw;
            }
            catch (SqlException ex)
            {
                _logger.LogError(ex, "Database error in {Operation} ProjectAssignment: ProjectId={ProjectId}", nameof(AddAsync), dto.ProjectId);
                throw new BusinessRuleException("Database is unavailable");
            }
            catch (DbUpdateException ex)
            {
                _logger.LogError(ex, "Database update error in {Operation} ProjectAssignment: ProjectId={ProjectId}", nameof(AddAsync), dto.ProjectId);
                throw new BusinessRuleException("Database operation failed");
            }
        }

        public async Task UpdateAsync(ProjectAssignmentDto dto)
        {
            try
            {
                if (dto.EndDate.HasValue && dto.EndDate < dto.JoinDate)
                    throw new ValidationException("End date cannot be before join date");

                var assignment = _mapper.Map<ProjectAssignment>(dto);
                await _repository.UpdateAsync(assignment);
            }
            catch (ValidationException ex)
            {
                _logger.LogWarning(ex, "Validation error in {Operation} ProjectAssignment: Id={Id}", nameof(UpdateAsync), dto.Id);
                throw;
            }
            catch (NotFoundException ex)
            {
                _logger.LogWarning(ex, "ProjectAssignment not found in {Operation}: Id={Id}", nameof(UpdateAsync), dto.Id);
                throw;
            }
            catch (SqlException ex)
            {
                _logger.LogError(ex, "Database error in {Operation} ProjectAssignment: Id={Id}", nameof(UpdateAsync), dto.Id);
                throw new BusinessRuleException("Database is unavailable");
            }
            catch (DbUpdateException ex)
            {
                _logger.LogError(ex, "Database update error in {Operation} ProjectAssignment: Id={Id}", nameof(UpdateAsync), dto.Id);
                throw new BusinessRuleException("Database operation failed");
            }
        }

        public async Task DeleteAsync(int id)
        {
            try
            {
                await _repository.DeleteAsync(id);
            }
            catch (NotFoundException ex)
            {
                _logger.LogWarning(ex, "ProjectAssignment not found in {Operation}: Id={Id}", nameof(DeleteAsync), id);
                throw;
            }
            catch (SqlException ex)
            {
                _logger.LogError(ex, "Database error in {Operation} ProjectAssignment: Id={Id}", nameof(DeleteAsync), id);
                throw new BusinessRuleException("Database is unavailable");
            }
            catch (DbUpdateException ex)
            {
                _logger.LogError(ex, "Database update error in {Operation} ProjectAssignment: Id={Id}", nameof(DeleteAsync), id);
                throw new BusinessRuleException("Database operation failed");
            }
        }
        #endregion

        #region Search
        public async Task<List<ProjectAssignmentDto>> SearchByEmployeeAsync(int employeeId)
        {
            try
            {
                var assignments = await _repository.SearchByEmployeeAsync(employeeId);
                return _mapper.Map<List<ProjectAssignmentDto>>(assignments);
            }
            catch (SqlException ex)
            {
                _logger.LogError(ex, "Database connection error in {Operation} ProjectAssignment: EmployeeId={EmployeeId}", nameof(SearchByEmployeeAsync), employeeId);
                throw new BusinessRuleException("Database is unavailable");
            }
        }

        public async Task<List<ProjectAssignmentDto>> SearchByProjectAsync(int projectId)
        {
            try
            {
                var assignments = await _repository.SearchByProjectAsync(projectId);
                return _mapper.Map<List<ProjectAssignmentDto>>(assignments);
            }
            catch (SqlException ex)
            {
                _logger.LogError(ex, "Database connection error in {Operation} ProjectAssignment: ProjectId={ProjectId}", nameof(SearchByProjectAsync), projectId);
                throw new BusinessRuleException("Database is unavailable");
            }
        }
        #endregion

        #region Filter
        public async Task<List<ProjectAssignmentDto>> GetByRoleAsync(string role)
        {
            try
            {
                var assignments = await _repository.GetByRoleAsync(role);
                return _mapper.Map<List<ProjectAssignmentDto>>(assignments);
            }
            catch (SqlException ex)
            {
                _logger.LogError(ex, "Database connection error in {Operation} ProjectAssignment: Role={Role}", nameof(GetByRoleAsync), role);
                throw new BusinessRuleException("Database is unavailable");
            }
        }

        public async Task<List<ProjectAssignmentDto>> GetByDateAsync(DateTime date)
        {
            try
            {
                var assignments = await _repository.GetByDateAsync(date);
                return _mapper.Map<List<ProjectAssignmentDto>>(assignments);
            }
            catch (SqlException ex)
            {
                _logger.LogError(ex, "Database connection error in {Operation} ProjectAssignment: Date={Date}", nameof(GetByDateAsync), date);
                throw new BusinessRuleException("Database is unavailable");
            }
        }
        #endregion
    }
}