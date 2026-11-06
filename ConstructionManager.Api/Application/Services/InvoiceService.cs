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
    public class InvoiceService : IInvoiceService
    {
        private readonly IInvoiceRepository _repository;
        private readonly IMapper _mapper;
        private readonly ILogger<InvoiceService> _logger;

        public InvoiceService(IInvoiceRepository repository, IMapper mapper, ILogger<InvoiceService> logger)
        {
            _repository = repository;
            _mapper = mapper;
            _logger = logger;
        }

        #region CRUD
        public async Task<List<InvoiceDto>> GetAllAsync()
        {
            try
            {
                var invoices = await _repository.GetAllAsync();
                return _mapper.Map<List<InvoiceDto>>(invoices);
            }
            catch (SqlException ex)
            {
                _logger.LogError(ex, "Database connection error in {Operation} Invoice", nameof(GetAllAsync));
                throw new BusinessRuleException("Database is unavailable");
            }
        }

        public async Task AddAsync(InvoiceDto dto)
        {
            try
            {
                if (await _repository.IsCodeDuplicateAsync(dto.Code))
                    throw new DuplicateException("Invoice code is duplicate");

                if (await _repository.IsTrackingCodeDuplicateAsync(dto.TrackingCode))
                    throw new DuplicateException("Invoice tracking code is duplicate");

                if (await _repository.IsProjectIdDuplicateAsync(dto.ProjectId))
                    throw new BusinessRuleException("Project already has an invoice");

                var invoice = _mapper.Map<Invoice>(dto);
                await _repository.AddAsync(invoice);
            }
            catch (DuplicateException ex)
            {
                _logger.LogWarning(ex, "Duplicate invoice in {Operation}: Code={Code}, TrackingCode={TrackingCode}",
                    nameof(AddAsync), dto.Code, dto.TrackingCode);
                throw;
            }
            catch (BusinessRuleException ex)
            {
                _logger.LogWarning(ex, "Business rule violation in {Operation} Invoice: ProjectId={ProjectId}", nameof(AddAsync), dto.ProjectId);
                throw;
            }
            catch (SqlException ex)
            {
                _logger.LogError(ex, "Database error in {Operation} Invoice: Code={Code}", nameof(AddAsync), dto.Code);
                throw new BusinessRuleException("Database is unavailable");
            }
            catch (DbUpdateException ex)
            {
                _logger.LogError(ex, "Database update error in {Operation} Invoice: Code={Code}", nameof(AddAsync), dto.Code);
                throw new BusinessRuleException("Database operation failed");
            }
        }

        public async Task UpdateAsync(InvoiceDto dto)
        {
            try
            {
                if (dto.Status == InvoiceStatus.Cancelled)
                    throw new OperationNotAllowedException("Cancelled invoice cannot be modified");

                var existing = await _repository.SearchByCodeAsync(dto.Code);
                if (existing != null && existing.Id != dto.Id)
                    throw new DuplicateException("Invoice code is duplicate");

                var invoice = _mapper.Map<Invoice>(dto);
                await _repository.UpdateAsync(invoice);
            }
            catch (OperationNotAllowedException ex)
            {
                _logger.LogWarning(ex, "Invalid operation in {Operation} Invoice: Id={Id}", nameof(UpdateAsync), dto.Id);
                throw;
            }
            catch (DuplicateException ex)
            {
                _logger.LogWarning(ex, "Duplicate invoice in {Operation}: Code={Code}, Id={Id}", nameof(UpdateAsync), dto.Code, dto.Id);
                throw;
            }
            catch (NotFoundException ex)
            {
                _logger.LogWarning(ex, "Invoice not found in {Operation}: Id={Id}", nameof(UpdateAsync), dto.Id);
                throw;
            }
            catch (SqlException ex)
            {
                _logger.LogError(ex, "Database error in {Operation} Invoice: Id={Id}", nameof(UpdateAsync), dto.Id);
                throw new BusinessRuleException("Database is unavailable");
            }
            catch (DbUpdateException ex)
            {
                _logger.LogError(ex, "Database update error in {Operation} Invoice: Id={Id}", nameof(UpdateAsync), dto.Id);
                throw new BusinessRuleException("Database operation failed");
            }
        }

        public async Task DeleteAsync(int id)
        {
            try
            {
                if (await _repository.HasPurchaseInvoicesAsync(id))
                    throw new BusinessRuleException("Invoice has purchase invoices");

                await _repository.DeleteAsync(id);
            }
            catch (BusinessRuleException ex)
            {
                _logger.LogWarning(ex, "Business rule violation in {Operation} Invoice: Id={Id}", nameof(DeleteAsync), id);
                throw;
            }
            catch (NotFoundException ex)
            {
                _logger.LogWarning(ex, "Invoice not found in {Operation}: Id={Id}", nameof(DeleteAsync), id);
                throw;
            }
            catch (SqlException ex)
            {
                _logger.LogError(ex, "Database error in {Operation} Invoice: Id={Id}", nameof(DeleteAsync), id);
                throw new BusinessRuleException("Database is unavailable");
            }
            catch (DbUpdateException ex)
            {
                _logger.LogError(ex, "Database update error in {Operation} Invoice: Id={Id}", nameof(DeleteAsync), id);
                throw new BusinessRuleException("Database operation failed");
            }
        }
        #endregion

        #region Search
        public async Task<InvoiceDto?> SearchByCodeAsync(string code)
        {
            try
            {
                var invoice = await _repository.SearchByCodeAsync(code);
                return _mapper.Map<InvoiceDto>(invoice);
            }
            catch (SqlException ex)
            {
                _logger.LogError(ex, "Database connection error in {Operation} Invoice: Code={Code}", nameof(SearchByCodeAsync), code);
                throw new BusinessRuleException("Database is unavailable");
            }
        }

        public async Task<InvoiceDto?> SearchByTrackingCodeAsync(string trackingCode)
        {
            try
            {
                var invoice = await _repository.SearchByTrackingCodeAsync(trackingCode);
                return _mapper.Map<InvoiceDto>(invoice);
            }
            catch (SqlException ex)
            {
                _logger.LogError(ex, "Database connection error in {Operation} Invoice: TrackingCode={TrackingCode}", nameof(SearchByTrackingCodeAsync), trackingCode);
                throw new BusinessRuleException("Database is unavailable");
            }
        }
        #endregion

        #region Filter
        public async Task<List<InvoiceDto>> GetByStatusAsync(InvoiceStatus status)
        {
            try
            {
                var invoices = await _repository.GetByStatusAsync(status);
                return _mapper.Map<List<InvoiceDto>>(invoices);
            }
            catch (SqlException ex)
            {
                _logger.LogError(ex, "Database connection error in {Operation} Invoice: Status={Status}", nameof(GetByStatusAsync), status);
                throw new BusinessRuleException("Database is unavailable");
            }
        }

        public async Task<List<InvoiceDto>> GetByDateRangeAsync(DateTime from, DateTime to)
        {
            try
            {
                var invoices = await _repository.GetByDateRangeAsync(from, to);
                return _mapper.Map<List<InvoiceDto>>(invoices);
            }
            catch (SqlException ex)
            {
                _logger.LogError(ex, "Database connection error in {Operation} Invoice: From={From}, To={To}",
                    nameof(GetByDateRangeAsync), from, to);
                throw new BusinessRuleException("Database is unavailable");
            }
        }
    }
}
    #endregion