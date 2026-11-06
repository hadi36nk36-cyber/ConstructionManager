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
    public class PurchaseInvoiceDetailService : IPurchaseInvoiceDetailService
    {
        private readonly IPurchaseInvoiceDetailRepository _repository;
        private readonly IMapper _mapper;
        private readonly ILogger<PurchaseInvoiceDetailService> _logger;

        public PurchaseInvoiceDetailService(IPurchaseInvoiceDetailRepository repository, IMapper mapper, ILogger<PurchaseInvoiceDetailService> logger)
        {
            _repository = repository;
            _mapper = mapper;
            _logger = logger;
        }

        #region CRUD
        public async Task<List<PurchaseInvoiceDetailDto>> GetAllAsync()
        {
            try
            {
                var details = await _repository.GetAllAsync();
                return _mapper.Map<List<PurchaseInvoiceDetailDto>>(details);
            }
            catch (SqlException ex)
            {
                _logger.LogError(ex, "Database connection error in {Operation} PurchaseInvoiceDetail", nameof(GetAllAsync));
                throw new BusinessRuleException("Database is unavailable");
            }
        }

        public async Task AddAsync(PurchaseInvoiceDetailDto dto)
        {
            try
            {
                if (dto.Quantity <= 0)
                    throw new ValidationException("Quantity must be greater than zero");

                if (dto.UnitPrice < 0)
                    throw new ValidationException("Unit price cannot be negative");

                var detail = _mapper.Map<PurchaseInvoiceDetail>(dto);
                await _repository.AddAsync(detail);
            }
            catch (ValidationException ex)
            {
                _logger.LogWarning(ex, "Validation error in {Operation} PurchaseInvoiceDetail: Material={Material}", nameof(AddAsync), dto.MaterialName);
                throw;
            }
            catch (SqlException ex)
            {
                _logger.LogError(ex, "Database error in {Operation} PurchaseInvoiceDetail: Material={Material}", nameof(AddAsync), dto.MaterialName);
                throw new BusinessRuleException("Database is unavailable");
            }
            catch (DbUpdateException ex)
            {
                _logger.LogError(ex, "Database update error in {Operation} PurchaseInvoiceDetail: Material={Material}", nameof(AddAsync), dto.MaterialName);
                throw new BusinessRuleException("Database operation failed");
            }
        }

        public async Task UpdateAsync(PurchaseInvoiceDetailDto dto)
        {
            try
            {
                if (dto.Quantity <= 0)
                    throw new ValidationException("Quantity must be greater than zero");

                if (dto.UnitPrice < 0)
                    throw new ValidationException("Unit price cannot be negative");

                var detail = _mapper.Map<PurchaseInvoiceDetail>(dto);
                await _repository.UpdateAsync(detail);
            }
            catch (ValidationException ex)
            {
                _logger.LogWarning(ex, "Validation error in {Operation} PurchaseInvoiceDetail: Id={Id}", nameof(UpdateAsync), dto.Id);
                throw;
            }
            catch (NotFoundException ex)
            {
                _logger.LogWarning(ex, "PurchaseInvoiceDetail not found in {Operation}: Id={Id}", nameof(UpdateAsync), dto.Id);
                throw;
            }
            catch (SqlException ex)
            {
                _logger.LogError(ex, "Database error in {Operation} PurchaseInvoiceDetail: Id={Id}", nameof(UpdateAsync), dto.Id);
                throw new BusinessRuleException("Database is unavailable");
            }
            catch (DbUpdateException ex)
            {
                _logger.LogError(ex, "Database update error in {Operation} PurchaseInvoiceDetail: Id={Id}", nameof(UpdateAsync), dto.Id);
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
                _logger.LogWarning(ex, "PurchaseInvoiceDetail not found in {Operation}: Id={Id}", nameof(DeleteAsync), id);
                throw;
            }
            catch (SqlException ex)
            {
                _logger.LogError(ex, "Database error in {Operation} PurchaseInvoiceDetail: Id={Id}", nameof(DeleteAsync), id);
                throw new BusinessRuleException("Database is unavailable");
            }
            catch (DbUpdateException ex)
            {
                _logger.LogError(ex, "Database update error in {Operation} PurchaseInvoiceDetail: Id={Id}", nameof(DeleteAsync), id);
                throw new BusinessRuleException("Database operation failed");
            }
        }
        #endregion
    }
}