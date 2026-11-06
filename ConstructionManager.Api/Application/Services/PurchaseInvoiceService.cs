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
    public class PurchaseInvoiceService : IPurchaseInvoiceService
    {
        private readonly IPurchaseInvoiceRepository _repository;
        private readonly IMapper _mapper;
        private readonly ILogger<PurchaseInvoiceService> _logger;

        public PurchaseInvoiceService(IPurchaseInvoiceRepository repository, IMapper mapper, ILogger<PurchaseInvoiceService> logger)
        {
            _repository = repository;
            _mapper = mapper;
            _logger = logger;
        }

        #region CRUD
        public async Task<List<PurchaseInvoiceDto>> GetAllAsync()
        {
            try
            {
                var invoices = await _repository.GetAllAsync();
                return _mapper.Map<List<PurchaseInvoiceDto>>(invoices);
            }
            catch (SqlException ex)
            {
                _logger.LogError(ex, "Database connection error in {Operation} PurchaseInvoice", nameof(GetAllAsync));
                throw new BusinessRuleException("Database is unavailable");
            }
        }

        public async Task AddAsync(PurchaseInvoiceDto dto)
        {
            try
            {
                if (await _repository.IsCodeDuplicateAsync(dto.Code))
                    throw new DuplicateException("Purchase invoice code is duplicate");

                if (await _repository.IsTrackingCodeDuplicateAsync(dto.TrackingCode))
                    throw new DuplicateException("Purchase invoice tracking code is duplicate");

                var invoice = _mapper.Map<PurchaseInvoice>(dto);
                await _repository.AddAsync(invoice);
            }
            catch (DuplicateException ex)
            {
                _logger.LogWarning(ex, "Duplicate purchase invoice in {Operation}: Code={Code}, TrackingCode={TrackingCode}",
                    nameof(AddAsync), dto.Code, dto.TrackingCode);
                throw;
            }
            catch (SqlException ex)
            {
                _logger.LogError(ex, "Database error in {Operation} PurchaseInvoice: Code={Code}", nameof(AddAsync), dto.Code);
                throw new BusinessRuleException("Database is unavailable");
            }
            catch (DbUpdateException ex)
            {
                _logger.LogError(ex, "Database update error in {Operation} PurchaseInvoice: Code={Code}", nameof(AddAsync), dto.Code);
                throw new BusinessRuleException("Database operation failed");
            }
        }

        public async Task UpdateAsync(PurchaseInvoiceDto dto)
        {
            try
            {
                if (dto.Status == PurchaseInvoiceStatus.Cancelled)
                    throw new OperationNotAllowedException("Cancelled purchase invoice cannot be modified");

                var existing = await _repository.SearchByCodeAsync(dto.Code);
                if (existing != null && existing.Id != dto.Id)
                    throw new DuplicateException("Purchase invoice code is duplicate");

                var invoice = _mapper.Map<PurchaseInvoice>(dto);
                await _repository.UpdateAsync(invoice);
            }
            catch (OperationNotAllowedException ex)
            {
                _logger.LogWarning(ex, "Invalid operation in {Operation} PurchaseInvoice: Id={Id}", nameof(UpdateAsync), dto.Id);
                throw;
            }
            catch (DuplicateException ex)
            {
                _logger.LogWarning(ex, "Duplicate purchase invoice in {Operation}: Code={Code}, Id={Id}", nameof(UpdateAsync), dto.Code, dto.Id);
                throw;
            }
            catch (NotFoundException ex)
            {
                _logger.LogWarning(ex, "Purchase invoice not found in {Operation}: Id={Id}", nameof(UpdateAsync), dto.Id);
                throw;
            }
            catch (SqlException ex)
            {
                _logger.LogError(ex, "Database error in {Operation} PurchaseInvoice: Id={Id}", nameof(UpdateAsync), dto.Id);
                throw new BusinessRuleException("Database is unavailable");
            }
            catch (DbUpdateException ex)
            {
                _logger.LogError(ex, "Database update error in {Operation} PurchaseInvoice: Id={Id}", nameof(UpdateAsync), dto.Id);
                throw new BusinessRuleException("Database operation failed");
            }
        }

        public async Task DeleteAsync(int id)
        {
            try
            {
                if (await _repository.HasDetailsAsync(id))
                    throw new BusinessRuleException("Purchase invoice has details");

                if (await _repository.HasPaymentsAsync(id))
                    throw new BusinessRuleException("Purchase invoice has payments");

                await _repository.DeleteAsync(id);
            }
            catch (BusinessRuleException ex)
            {
                _logger.LogWarning(ex, "Business rule violation in {Operation} PurchaseInvoice: Id={Id}", nameof(DeleteAsync), id);
                throw;
            }
            catch (NotFoundException ex)
            {
                _logger.LogWarning(ex, "Purchase invoice not found in {Operation}: Id={Id}", nameof(DeleteAsync), id);
                throw;
            }
            catch (SqlException ex)
            {
                _logger.LogError(ex, "Database error in {Operation} PurchaseInvoice: Id={Id}", nameof(DeleteAsync), id);
                throw new BusinessRuleException("Database is unavailable");
            }
            catch (DbUpdateException ex)
            {
                _logger.LogError(ex, "Database update error in {Operation} PurchaseInvoice: Id={Id}", nameof(DeleteAsync), id);
                throw new BusinessRuleException("Database operation failed");
            }
        }
        #endregion

        #region Search
        public async Task<PurchaseInvoiceDto?> SearchByCodeAsync(string code)
        {
            try
            {
                var invoice = await _repository.SearchByCodeAsync(code);
                return _mapper.Map<PurchaseInvoiceDto>(invoice);
            }
            catch (SqlException ex)
            {
                _logger.LogError(ex, "Database connection error in {Operation} PurchaseInvoice: Code={Code}", nameof(SearchByCodeAsync), code);
                throw new BusinessRuleException("Database is unavailable");
            }
        }

        public async Task<PurchaseInvoiceDto?> SearchByTrackingCodeAsync(string trackingCode)
        {
            try
            {
                var invoice = await _repository.SearchByTrackingCodeAsync(trackingCode);
                return _mapper.Map<PurchaseInvoiceDto>(invoice);
            }
            catch (SqlException ex)
            {
                _logger.LogError(ex, "Database connection error in {Operation} PurchaseInvoice: TrackingCode={TrackingCode}", nameof(SearchByTrackingCodeAsync), trackingCode);
                throw new BusinessRuleException("Database is unavailable");
            }
        }
        #endregion

        #region Filter
        public async Task<List<PurchaseInvoiceDto>> GetByInvoiceDateAsync(DateTime date)
        {
            try
            {
                var invoices = await _repository.GetByInvoiceDateAsync(date);
                return _mapper.Map<List<PurchaseInvoiceDto>>(invoices);
            }
            catch (SqlException ex)
            {
                _logger.LogError(ex, "Database connection error in {Operation} PurchaseInvoice: Date={Date}", nameof(GetByInvoiceDateAsync), date);
                throw new BusinessRuleException("Database is unavailable");
            }
        }
        #endregion
    }
}