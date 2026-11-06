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
    public class PaymentService : IPaymentService
    {
        private readonly IPaymentRepository _repository;
        private readonly IMapper _mapper;
        private readonly ILogger<PaymentService> _logger;

        public PaymentService(IPaymentRepository repository, IMapper mapper, ILogger<PaymentService> logger)
        {
            _repository = repository;
            _mapper = mapper;
            _logger = logger;
        }

        #region CRUD
        public async Task<List<PaymentDto>> GetAllAsync()
        {
            try
            {
                var payments = await _repository.GetAllAsync();
                return _mapper.Map<List<PaymentDto>>(payments);
            }
            catch (SqlException ex)
            {
                _logger.LogError(ex, "Database connection error in {Operation} Payment", nameof(GetAllAsync));
                throw new BusinessRuleException("Database is unavailable");
            }
        }

        public async Task AddAsync(PaymentDto dto)
        {
            try
            {
                if (await _repository.IsCodeDuplicateAsync(dto.Code))
                    throw new DuplicateException("Payment code is duplicate");

                if (await _repository.IsTrackingCodeDuplicateAsync(dto.TrackingCode))
                    throw new DuplicateException("Payment tracking code is duplicate");

                if (dto.Amount <= 0)
                    throw new ValidationException("Amount must be greater than zero");

                if (!dto.EmployeeId.HasValue && !dto.PurchaseInvoiceId.HasValue && !dto.EquipmentId.HasValue)
                    throw new BusinessRuleException("Payment must have at least one reference");

                var payment = _mapper.Map<Payment>(dto);

                if (dto.EmployeeId.HasValue)
                    payment.PaymentType = PaymentType.Salary;
                else if (dto.PurchaseInvoiceId.HasValue)
                    payment.PaymentType = PaymentType.PurchaseMaterial;
                else if (dto.EquipmentId.HasValue)
                    payment.PaymentType = PaymentType.Equipment;
                else
                    payment.PaymentType = PaymentType.Other;

                await _repository.AddAsync(payment);
            }
            catch (DuplicateException ex)
            {
                _logger.LogWarning(ex, "Duplicate payment in {Operation}: Code={Code}, TrackingCode={TrackingCode}",
                    nameof(AddAsync), dto.Code, dto.TrackingCode);
                throw;
            }
            catch (ValidationException ex)
            {
                _logger.LogWarning(ex, "Validation error in {Operation} Payment: Code={Code}", nameof(AddAsync), dto.Code);
                throw;
            }
            catch (BusinessRuleException ex)
            {
                _logger.LogWarning(ex, "Business rule violation in {Operation} Payment: Code={Code}", nameof(AddAsync), dto.Code);
                throw;
            }
            catch (SqlException ex)
            {
                _logger.LogError(ex, "Database error in {Operation} Payment: Code={Code}", nameof(AddAsync), dto.Code);
                throw new BusinessRuleException("Database is unavailable");
            }
            catch (DbUpdateException ex)
            {
                _logger.LogError(ex, "Database update error in {Operation} Payment: Code={Code}", nameof(AddAsync), dto.Code);
                throw new BusinessRuleException("Database operation failed");
            }
        }

        public async Task UpdateAsync(PaymentDto dto)
        {
            try
            {
                if (dto.Amount <= 0)
                    throw new ValidationException("Amount must be greater than zero");

                var payment = _mapper.Map<Payment>(dto);
                await _repository.UpdateAsync(payment);
            }
            catch (ValidationException ex)
            {
                _logger.LogWarning(ex, "Validation error in {Operation} Payment: Id={Id}", nameof(UpdateAsync), dto.Id);
                throw;
            }
            catch (NotFoundException ex)
            {
                _logger.LogWarning(ex, "Payment not found in {Operation}: Id={Id}", nameof(UpdateAsync), dto.Id);
                throw;
            }
            catch (SqlException ex)
            {
                _logger.LogError(ex, "Database error in {Operation} Payment: Id={Id}", nameof(UpdateAsync), dto.Id);
                throw new BusinessRuleException("Database is unavailable");
            }
            catch (DbUpdateException ex)
            {
                _logger.LogError(ex, "Database update error in {Operation} Payment: Id={Id}", nameof(UpdateAsync), dto.Id);
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
                _logger.LogWarning(ex, "Payment not found in {Operation}: Id={Id}", nameof(DeleteAsync), id);
                throw;
            }
            catch (SqlException ex)
            {
                _logger.LogError(ex, "Database error in {Operation} Payment: Id={Id}", nameof(DeleteAsync), id);
                throw new BusinessRuleException("Database is unavailable");
            }
            catch (DbUpdateException ex)
            {
                _logger.LogError(ex, "Database update error in {Operation} Payment: Id={Id}", nameof(DeleteAsync), id);
                throw new BusinessRuleException("Database operation failed");
            }
        }
        #endregion

        #region Search
        public async Task<PaymentDto?> SearchByCodeAsync(string code)
        {
            try
            {
                var payment = await _repository.SearchByCodeAsync(code);
                return _mapper.Map<PaymentDto>(payment);
            }
            catch (SqlException ex)
            {
                _logger.LogError(ex, "Database connection error in {Operation} Payment: Code={Code}", nameof(SearchByCodeAsync), code);
                throw new BusinessRuleException("Database is unavailable");
            }
        }

        public async Task<PaymentDto?> SearchByTrackingCodeAsync(string trackingCode)
        {
            try
            {
                var payment = await _repository.SearchByTrackingCodeAsync(trackingCode);
                return _mapper.Map<PaymentDto>(payment);
            }
            catch (SqlException ex)
            {
                _logger.LogError(ex, "Database connection error in {Operation} Payment: TrackingCode={TrackingCode}", nameof(SearchByTrackingCodeAsync), trackingCode);
                throw new BusinessRuleException("Database is unavailable");
            }
        }
        #endregion

        #region Filter
        public async Task<List<PaymentDto>> GetByTypeAsync(PaymentType type)
        {
            try
            {
                var payments = await _repository.GetByTypeAsync(type);
                return _mapper.Map<List<PaymentDto>>(payments);
            }
            catch (SqlException ex)
            {
                _logger.LogError(ex, "Database connection error in {Operation} Payment: Type={Type}", nameof(GetByTypeAsync), type);
                throw new BusinessRuleException("Database is unavailable");
            }
        }

        public async Task<List<PaymentDto>> GetByDateAsync(DateTime date)
        {
            try
            {
                var payments = await _repository.GetByDateAsync(date);
                return _mapper.Map<List<PaymentDto>>(payments);
            }
            catch (SqlException ex)
            {
                _logger.LogError(ex, "Database connection error in {Operation} Payment: Date={Date}", nameof(GetByDateAsync), date);
                throw new BusinessRuleException("Database is unavailable");
            }
        }
        #endregion
    }
}