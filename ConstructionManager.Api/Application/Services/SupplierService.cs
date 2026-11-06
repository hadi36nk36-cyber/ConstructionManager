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
    public class SupplierService : ISupplierService
    {
        private readonly ISupplierRepository _repository;
        private readonly IMapper _mapper;
        private readonly ILogger<SupplierService> _logger;

        public SupplierService(ISupplierRepository repository, IMapper mapper, ILogger<SupplierService> logger)
        {
            _repository = repository;
            _mapper = mapper;
            _logger = logger;
        }

        #region CRUD
        public async Task<List<SupplierDto>> GetAllAsync()
        {
            try
            {
                var suppliers = await _repository.GetAllAsync();
                return _mapper.Map<List<SupplierDto>>(suppliers);
            }
            catch (SqlException ex)
            {
                _logger.LogError(ex, "Database connection error in {Operation} Supplier", nameof(GetAllAsync));
                throw new BusinessRuleException("Database is unavailable");
            }
        }

        public async Task AddAsync(SupplierDto dto)
        {
            try
            {
                if (await _repository.IsCodeDuplicateAsync(dto.Code))
                    throw new DuplicateException("Supplier code is duplicate");

                if (await _repository.IsPhoneDuplicateAsync(dto.Phone1))
                    throw new DuplicateException("Supplier phone is duplicate");

                var supplier = _mapper.Map<Supplier>(dto);
                await _repository.AddAsync(supplier);
            }
            catch (DuplicateException ex)
            {
                _logger.LogWarning(ex, "Duplicate supplier in {Operation}: Code={Code}, Phone={Phone}",
                    nameof(AddAsync), dto.Code, dto.Phone1);
                throw;
            }
            catch (SqlException ex)
            {
                _logger.LogError(ex, "Database error in {Operation} Supplier: Code={Code}", nameof(AddAsync), dto.Code);
                throw new BusinessRuleException("Database is unavailable");
            }
            catch (DbUpdateException ex)
            {
                _logger.LogError(ex, "Database update error in {Operation} Supplier: Code={Code}", nameof(AddAsync), dto.Code);
                throw new BusinessRuleException("Database operation failed");
            }
        }

        public async Task UpdateAsync(SupplierDto dto)
        {
            try
            {
                var existing = await _repository.SearchByCodeAsync(dto.Code);
                if (existing != null && existing.Id != dto.Id)
                    throw new DuplicateException("Supplier code is duplicate");

                var supplier = _mapper.Map<Supplier>(dto);
                await _repository.UpdateAsync(supplier);
            }
            catch (DuplicateException ex)
            {
                _logger.LogWarning(ex, "Duplicate supplier in {Operation}: Code={Code}, Id={Id}",
                    nameof(UpdateAsync), dto.Code, dto.Id);
                throw;
            }
            catch (NotFoundException ex)
            {
                _logger.LogWarning(ex, "Supplier not found in {Operation}: Id={Id}", nameof(UpdateAsync), dto.Id);
                throw;
            }
            catch (SqlException ex)
            {
                _logger.LogError(ex, "Database error in {Operation} Supplier: Id={Id}", nameof(UpdateAsync), dto.Id);
                throw new BusinessRuleException("Database is unavailable");
            }
            catch (DbUpdateException ex)
            {
                _logger.LogError(ex, "Database update error in {Operation} Supplier: Id={Id}", nameof(UpdateAsync), dto.Id);
                throw new BusinessRuleException("Database operation failed");
            }
        }

        public async Task DeleteAsync(int id)
        {
            try
            {
                if (await _repository.HasPurchaseInvoicesAsync(id))
                    throw new BusinessRuleException("Supplier has purchase invoices");

                await _repository.DeleteAsync(id);
            }
            catch (BusinessRuleException ex)
            {
                _logger.LogWarning(ex, "Business rule violation in {Operation} Supplier: Id={Id}", nameof(DeleteAsync), id);
                throw;
            }
            catch (NotFoundException ex)
            {
                _logger.LogWarning(ex, "Supplier not found in {Operation}: Id={Id}", nameof(DeleteAsync), id);
                throw;
            }
            catch (SqlException ex)
            {
                _logger.LogError(ex, "Database error in {Operation} Supplier: Id={Id}", nameof(DeleteAsync), id);
                throw new BusinessRuleException("Database is unavailable");
            }
            catch (DbUpdateException ex)
            {
                _logger.LogError(ex, "Database update error in {Operation} Supplier: Id={Id}", nameof(DeleteAsync), id);
                throw new BusinessRuleException("Database operation failed");
            }
        }
        #endregion

        #region Search
        public async Task<SupplierDto?> SearchByCodeAsync(string code)
        {
            try
            {
                var supplier = await _repository.SearchByCodeAsync(code);
                return _mapper.Map<SupplierDto>(supplier);
            }
            catch (SqlException ex)
            {
                _logger.LogError(ex, "Database connection error in {Operation} Supplier: Code={Code}", nameof(SearchByCodeAsync), code);
                throw new BusinessRuleException("Database is unavailable");
            }
        }

        public async Task<List<SupplierDto>> SearchByNameAsync(string name)
        {
            try
            {
                var suppliers = await _repository.SearchByNameAsync(name);
                return _mapper.Map<List<SupplierDto>>(suppliers);
            }
            catch (SqlException ex)
            {
                _logger.LogError(ex, "Database connection error in {Operation} Supplier: Name={Name}", nameof(SearchByNameAsync), name);
                throw new BusinessRuleException("Database is unavailable");
            }
        }

        public async Task<List<SupplierDto>> SearchByPhoneAsync(string phone)
        {
            try
            {
                var suppliers = await _repository.SearchByPhoneAsync(phone);
                return _mapper.Map<List<SupplierDto>>(suppliers);
            }
            catch (SqlException ex)
            {
                _logger.LogError(ex, "Database connection error in {Operation} Supplier: Phone={Phone}", nameof(SearchByPhoneAsync), phone);
                throw new BusinessRuleException("Database is unavailable");
            }
        }
        #endregion
    }
}