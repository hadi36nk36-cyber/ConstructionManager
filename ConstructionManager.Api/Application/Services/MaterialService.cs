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
    public class MaterialService : IMaterialService
    {
        private readonly IMaterialRepository _repository;
        private readonly IMapper _mapper;
        private readonly ILogger<MaterialService> _logger;

        public MaterialService(IMaterialRepository repository, IMapper mapper, ILogger<MaterialService> logger)
        {
            _repository = repository;
            _mapper = mapper;
            _logger = logger;
        }

        #region CRUD
        public async Task<List<MaterialDto>> GetAllAsync()
        {
            try
            {
                var materials = await _repository.GetAllAsync();
                return _mapper.Map<List<MaterialDto>>(materials);
            }
            catch (SqlException ex)
            {
                _logger.LogError(ex, "Database connection error in {Operation} Material", nameof(GetAllAsync));
                throw new BusinessRuleException("Database is unavailable");
            }
        }

        public async Task AddAsync(MaterialDto dto)
        {
            try
            {
                if (await _repository.IsCodeDuplicateAsync(dto.Code))
                    throw new DuplicateException("Material code is duplicate");

                if (dto.Quantity < 0)
                    throw new ValidationException("Quantity cannot be negative");

                var material = _mapper.Map<Material>(dto);
                await _repository.AddAsync(material);
            }
            catch (DuplicateException ex)
            {
                _logger.LogWarning(ex, "Duplicate material in {Operation}: Code={Code}", nameof(AddAsync), dto.Code);
                throw;
            }
            catch (ValidationException ex)
            {
                _logger.LogWarning(ex, "Validation error in {Operation} Material: Code={Code}", nameof(AddAsync), dto.Code);
                throw;
            }
            catch (SqlException ex)
            {
                _logger.LogError(ex, "Database error in {Operation} Material: Code={Code}", nameof(AddAsync), dto.Code);
                throw new BusinessRuleException("Database is unavailable");
            }
            catch (DbUpdateException ex)
            {
                _logger.LogError(ex, "Database update error in {Operation} Material: Code={Code}", nameof(AddAsync), dto.Code);
                throw new BusinessRuleException("Database operation failed");
            }
        }

        public async Task UpdateAsync(MaterialDto dto)
        {
            try
            {
                var existing = await _repository.SearchByCodeAsync(dto.Code);
                if (existing != null && existing.Id != dto.Id)
                    throw new DuplicateException("Material code is duplicate");

                if (dto.Quantity < 0)
                    throw new ValidationException("Quantity cannot be negative");

                var material = _mapper.Map<Material>(dto);
                await _repository.UpdateAsync(material);
            }
            catch (DuplicateException ex)
            {
                _logger.LogWarning(ex, "Duplicate material in {Operation}: Code={Code}, Id={Id}", nameof(UpdateAsync), dto.Code, dto.Id);
                throw;
            }
            catch (ValidationException ex)
            {
                _logger.LogWarning(ex, "Validation error in {Operation} Material: Id={Id}", nameof(UpdateAsync), dto.Id);
                throw;
            }
            catch (NotFoundException ex)
            {
                _logger.LogWarning(ex, "Material not found in {Operation}: Id={Id}", nameof(UpdateAsync), dto.Id);
                throw;
            }
            catch (SqlException ex)
            {
                _logger.LogError(ex, "Database error in {Operation} Material: Id={Id}", nameof(UpdateAsync), dto.Id);
                throw new BusinessRuleException("Database is unavailable");
            }
            catch (DbUpdateException ex)
            {
                _logger.LogError(ex, "Database update error in {Operation} Material: Id={Id}", nameof(UpdateAsync), dto.Id);
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
                _logger.LogWarning(ex, "Material not found in {Operation}: Id={Id}", nameof(DeleteAsync), id);
                throw;
            }
            catch (SqlException ex)
            {
                _logger.LogError(ex, "Database error in {Operation} Material: Id={Id}", nameof(DeleteAsync), id);
                throw new BusinessRuleException("Database is unavailable");
            }
            catch (DbUpdateException ex)
            {
                _logger.LogError(ex, "Database update error in {Operation} Material: Id={Id}", nameof(DeleteAsync), id);
                throw new BusinessRuleException("Database operation failed");
            }
        }
        #endregion

        #region Search
        public async Task<MaterialDto?> SearchByCodeAsync(string code)
        {
            try
            {
                var material = await _repository.SearchByCodeAsync(code);
                return _mapper.Map<MaterialDto>(material);
            }
            catch (SqlException ex)
            {
                _logger.LogError(ex, "Database connection error in {Operation} Material: Code={Code}", nameof(SearchByCodeAsync), code);
                throw new BusinessRuleException("Database is unavailable");
            }
        }

        public async Task<List<MaterialDto>> SearchByNameAsync(string name)
        {
            try
            {
                var materials = await _repository.SearchByNameAsync(name);
                return _mapper.Map<List<MaterialDto>>(materials);
            }
            catch (SqlException ex)
            {
                _logger.LogError(ex, "Database connection error in {Operation} Material: Name={Name}", nameof(SearchByNameAsync), name);
                throw new BusinessRuleException("Database is unavailable");
            }
        }
        #endregion

        #region Filter
        public async Task<List<MaterialDto>> GetByCategoryAsync(string category)
        {
            try
            {
                var materials = await _repository.GetByCategoryAsync(category);
                return _mapper.Map<List<MaterialDto>>(materials);
            }
            catch (SqlException ex)
            {
                _logger.LogError(ex, "Database connection error in {Operation} Material: Category={Category}", nameof(GetByCategoryAsync), category);
                throw new BusinessRuleException("Database is unavailable");
            }
        }
        #endregion
    }
}