using AutoMapper;
using ConstructionManager.Api.Application.DTOs;
using ConstructionManager.Api.Application.Interfaces.Repositories;
using ConstructionManager.Api.Application.Interfaces.Services;
using ConstructionManager.Api.Domain.Entities;
using ConstructionManager.Api.Domain.Enums;
using ConstructionManager.Api.Domain.Exceptions;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace ConstructionManager.Api.Application.Services
{
    public class ProjectService : IProjectService
    {
        private readonly IProjectRepository _repository;
        private readonly IMapper _mapper;
        private readonly ILogger<ProjectService> _logger;

        public ProjectService(IProjectRepository repository, IMapper mapper, ILogger<ProjectService> logger)
        {
            _repository = repository;
            _mapper = mapper;
            _logger = logger;
        }

        #region CRUD
        public async Task<List<ProjectDto>> GetAllAsync()
        {
            try
            {
                var projects = await _repository.GetAllAsync();
                return _mapper.Map<List<ProjectDto>>(projects);
            }
            catch (SqlException ex)
            {
                _logger.LogError(ex, "Database connection error in {Operation} Project", nameof(GetAllAsync));
                throw new BusinessRuleException("Database is unavailable");
            }
        }

        public async Task AddAsync(ProjectDto dto)
        {
            try
            {
                if (await _repository.IsCodeDuplicateAsync(dto.Code))
                    throw new DuplicateException("Project code is duplicate");

                if (dto.EndDate < dto.StartDate)
                    throw new ValidationException("End date cannot be before start date");

                var project = _mapper.Map<Project>(dto);
                await _repository.AddAsync(project);
            }
            catch (DuplicateException ex)
            {
                _logger.LogWarning(ex, "Duplicate project in {Operation}: Code={Code}", nameof(AddAsync), dto.Code);
                throw;
            }
            catch (ValidationException ex)
            {
                _logger.LogWarning(ex, "Validation error in {Operation} Project: Code={Code}", nameof(AddAsync), dto.Code);
                throw;
            }
            catch (SqlException ex)
            {
                _logger.LogError(ex, "Database error in {Operation} Project: Code={Code}", nameof(AddAsync), dto.Code);
                throw new BusinessRuleException("Database is unavailable");
            }
            catch (DbUpdateException ex)
            {
                _logger.LogError(ex, "Database update error in {Operation} Project: Code={Code}", nameof(AddAsync), dto.Code);
                throw new BusinessRuleException("Database operation failed");
            }
        }

        public async Task UpdateAsync(ProjectDto dto)
        {
            try
            {
                var existing = await _repository.SearchByCodeAsync(dto.Code);
                if (existing != null && existing.Id != dto.Id)
                    throw new DuplicateException("Project code is duplicate");

                if (dto.EndDate < dto.StartDate)
                    throw new ValidationException("End date cannot be before start date");

                var project = _mapper.Map<Project>(dto);
                await _repository.UpdateAsync(project);
            }
            catch (DuplicateException ex)
            {
                _logger.LogWarning(ex, "Duplicate project in {Operation}: Code={Code}, Id={Id}", nameof(UpdateAsync), dto.Code, dto.Id);
                throw;
            }
            catch (ValidationException ex)
            {
                _logger.LogWarning(ex, "Validation error in {Operation} Project: Id={Id}", nameof(UpdateAsync), dto.Id);
                throw;
            }
            catch (NotFoundException ex)
            {
                _logger.LogWarning(ex, "Project not found in {Operation}: Id={Id}", nameof(UpdateAsync), dto.Id);
                throw;
            }
            catch (SqlException ex)
            {
                _logger.LogError(ex, "Database error in {Operation} Project: Id={Id}", nameof(UpdateAsync), dto.Id);
                throw new BusinessRuleException("Database is unavailable");
            }
            catch (DbUpdateException ex)
            {
                _logger.LogError(ex, "Database update error in {Operation} Project: Id={Id}", nameof(UpdateAsync), dto.Id);
                throw new BusinessRuleException("Database operation failed");
            }
        }

        public async Task DeleteAsync(int id)
        {
            try
            {
                if (await _repository.HasProjectAssignmentsAsync(id))
                    throw new BusinessRuleException("Project has assignments");

                if (await _repository.HasInvoicesAsync(id))
                    throw new BusinessRuleException("Project has invoices");

                if (await _repository.HasPurchaseInvoicesAsync(id))
                    throw new BusinessRuleException("Project has purchase invoices");

                await _repository.DeleteAsync(id);
            }
            catch (BusinessRuleException ex)
            {
                _logger.LogWarning(ex, "Business rule violation in {Operation} Project: Id={Id}", nameof(DeleteAsync), id);
                throw;
            }
            catch (NotFoundException ex)
            {
                _logger.LogWarning(ex, "Project not found in {Operation}: Id={Id}", nameof(DeleteAsync), id);
                throw;
            }
            catch (SqlException ex)
            {
                _logger.LogError(ex, "Database error in {Operation} Project: Id={Id}", nameof(DeleteAsync), id);
                throw new BusinessRuleException("Database is unavailable");
            }
            catch (DbUpdateException ex)
            {
                _logger.LogError(ex, "Database update error in {Operation} Project: Id={Id}", nameof(DeleteAsync), id);
                throw new BusinessRuleException("Database operation failed");
            }
        }
        #endregion

        #region Search
        public async Task<ProjectDto?> SearchByCodeAsync(string code)
        {
            try
            {
                var project = await _repository.SearchByCodeAsync(code);
                return _mapper.Map<ProjectDto>(project);
            }
            catch (SqlException ex)
            {
                _logger.LogError(ex, "Database connection error in {Operation} Project: Code={Code}", nameof(SearchByCodeAsync), code);
                throw new BusinessRuleException("Database is unavailable");
            }
        }

        public async Task<List<ProjectDto>> SearchByNameAsync(string name)
        {
            try
            {
                var projects = await _repository.SearchByNameAsync(name);
                return _mapper.Map<List<ProjectDto>>(projects);
            }
            catch (SqlException ex)
            {
                _logger.LogError(ex, "Database connection error in {Operation} Project: Name={Name}", nameof(SearchByNameAsync), name);
                throw new BusinessRuleException("Database is unavailable");
            }
        }
        #endregion

        #region Filter
        public async Task<List<ProjectDto>> GetByStatusAsync(ProjectStatus status)
        {
            try
            {
                var projects = await _repository.GetByStatusAsync(status);
                return _mapper.Map<List<ProjectDto>>(projects);
            }
            catch (SqlException ex)
            {
                _logger.LogError(ex, "Database connection error in {Operation} Project: Status={Status}", nameof(GetByStatusAsync), status);
                throw new BusinessRuleException("Database is unavailable");
            }
        }

        public async Task<List<ProjectDto>> GetByCustomerAsync(int customerId)
        {
            try
            {
                var projects = await _repository.GetByCustomerAsync(customerId);
                return _mapper.Map<List<ProjectDto>>(projects);
            }
            catch (SqlException ex)
            {
                _logger.LogError(ex, "Database connection error in {Operation} Project: CustomerId={CustomerId}", nameof(GetByCustomerAsync), customerId);
                throw new BusinessRuleException("Database is unavailable");
            }
        }

        public async Task<List<ProjectDto>> GetByDateRangeAsync(DateTime from, DateTime to)
        {
            try
            {
                var projects = await _repository.GetByDateRangeAsync(from, to);
                return _mapper.Map<List<ProjectDto>>(projects);
            }
            catch (SqlException ex)
            {
                _logger.LogError(ex, "Database connection error in {Operation} Project: From={From}, To={To}", nameof(GetByDateRangeAsync), from, to);
                throw new BusinessRuleException("Database is unavailable");
            }
        }
        #endregion
    }
}