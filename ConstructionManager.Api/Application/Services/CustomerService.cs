using AutoMapper;
using ConstructionManager.Api.Application.DTOs;
using ConstructionManager.Api.Application.Interfaces.Repositories;
using ConstructionManager.Api.Application.Interfaces.Services;
using ConstructionManager.Api.Domain.Entities;
using ConstructionManager.Api.Domain.Enums;
using ConstructionManager.Api.Domain.Exceptions;
using Npgsql;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace ConstructionManager.Api.Application.Services
{
    public class CustomerService : ICustomerService
    {
        private readonly ICustomerRepository _repository;
        private readonly IMapper _mapper;
        private readonly ILogger<CustomerService> _logger;

        public CustomerService(
            ICustomerRepository repository,
            IMapper mapper,
            ILogger<CustomerService> logger)
        {
            _repository = repository;
            _mapper = mapper;
            _logger = logger;
        }

        #region CRUD
        public async Task<List<CustomerDto>> GetAllAsync()
        {
            try
            {
                var customers = await _repository.GetAllAsync();
                return _mapper.Map<List<CustomerDto>>(customers);
            }
            catch (NpgsqlException ex)
            {
                _logger.LogError(ex, "Database connection error in {Operation} Customer", 
                    nameof(GetAllAsync));
                throw new BusinessRuleException("Database is unavailable");
            }
        }

        public async Task AddAsync(CustomerDto dto)
        {
            try
            {
                if (await _repository.IsCodeDuplicateAsync(dto.Code))
                    throw new DuplicateException("Customer code is duplicate");

                if (await _repository.IsPhoneDuplicateAsync(dto.Phone1))
                    throw new DuplicateException("Customer phone is duplicate");

                var customer = _mapper.Map<Customer>(dto);
                await _repository.AddAsync(customer);
            }
            catch (DuplicateException ex)
            {
                _logger.LogWarning(ex, "Duplicate customer in {Operation}: Code={CustomerCode}, Phone={CustomerPhone}",
                    nameof(AddAsync), dto.Code, dto.Phone1);
                throw;
            }
            catch (NpgsqlException ex)
            {
                _logger.LogError(ex, "Database error in {Operation} Customer: Code={CustomerCode}",
                    nameof(AddAsync), dto.Code);
                throw new BusinessRuleException("Database is unavailable");
            }
            catch (DbUpdateException ex)
            {
                _logger.LogError(ex, "Database update error in {Operation} Customer: Code={CustomerCode}",
                    nameof(AddAsync), dto.Code);
                throw new BusinessRuleException("Database operation failed");
            }
        }

        public async Task UpdateAsync(CustomerDto dto)
        {
            try
            {
                var existing = await _repository.SearchByCodeAsync(dto.Code);
                if (existing != null && existing.Id != dto.Id)
                    throw new DuplicateException("Customer code is duplicate");

                var customer = _mapper.Map<Customer>(dto);
                await _repository.UpdateAsync(customer);
            }
            catch (DuplicateException ex)
            {
                _logger.LogWarning(ex, "Duplicate customer in {Operation}: Code={CustomerCode}, Id={CustomerId}",
                    nameof(UpdateAsync), dto.Code, dto.Id);
                throw;
            }
            catch (NotFoundException ex)
            {
                _logger.LogWarning(ex, "Customer not found in {Operation}: Id={CustomerId}",
                    nameof(UpdateAsync), dto.Id);   // ← UpdateAsync
                throw;
            }
            catch (NpgsqlException ex)
            {
                _logger.LogError(ex, "Database error in {Operation} Customer: Id={CustomerId}",
                    nameof(UpdateAsync), dto.Id);
                throw new BusinessRuleException("Database is unavailable");
            }
            catch (DbUpdateException ex)
            {
                _logger.LogError(ex, "Database update error in {Operation} Customer: Id={CustomerId}",
                    nameof(UpdateAsync), dto.Id);
                throw new BusinessRuleException("Database operation failed");
            }
        }

        public async Task DeleteAsync(int id)
        {
            try
            {
                if (await _repository.HasProjectsAsync(id))
                    throw new BusinessRuleException("Customer has projects");

                if (await _repository.HasInvoicesAsync(id))
                    throw new BusinessRuleException("Customer has invoices");

                await _repository.DeleteAsync(id);
            }
            catch (BusinessRuleException ex)
            {
                _logger.LogWarning(ex, "Business rule violation in {Operation} Customer: Id={CustomerId}",
                    nameof(DeleteAsync), id);
                throw;
            }
            catch (NotFoundException ex)
            {
                _logger.LogWarning(ex, "Customer not found in {Operation}: Id={CustomerId}",
                    nameof(DeleteAsync), id);
                throw;
            }
            catch (NpgsqlException ex)
            {
                _logger.LogError(ex, "Database error in {Operation} Customer: Id={CustomerId}",
                    nameof(DeleteAsync), id);
                throw new BusinessRuleException("Database is unavailable");
            }
            catch (DbUpdateException ex)
            {
                _logger.LogError(ex, "Database update error in {Operation} Customer: Id={CustomerId}",
                    nameof(DeleteAsync), id);
                throw new BusinessRuleException("Database operation failed");
            }
        }
        #endregion

        #region Search
        public async Task<CustomerDto?> SearchByCodeAsync(string code)
        {
            try
            {
                var customer = await _repository.SearchByCodeAsync(code);
                return _mapper.Map<CustomerDto>(customer);
            }
            catch (NpgsqlException ex)
            {
                _logger.LogError(ex, "Database connection error in {Operation} Customer: Code={CustomerCode}",
                    nameof(SearchByCodeAsync), code);
                throw new BusinessRuleException("Database is unavailable");
            }
        }

        public async Task<List<CustomerDto>> SearchByNameAsync(string name)
        {
            try
            {
                var customers = await _repository.SearchByNameAsync(name);
                return _mapper.Map<List<CustomerDto>>(customers);
            }
            catch (NpgsqlException ex)
            {
                _logger.LogError(ex, "Database connection error in {Operation} Customer: Name={CustomerName}",
                    nameof(SearchByNameAsync), name);
                throw new BusinessRuleException("Database is unavailable");
            }
        }

        public async Task<List<CustomerDto>> SearchByPhoneAsync(string phone)
        {
            try
            {
                var customers = await _repository.SearchByPhoneAsync(phone);
                return _mapper.Map<List<CustomerDto>>(customers);
            }
            catch (NpgsqlException ex)
            {
                _logger.LogError(ex, "Database connection error in {Operation} Customer: Phone={CustomerPhone}",
                    nameof(SearchByPhoneAsync), phone);
                throw new BusinessRuleException("Database is unavailable");
            }
        }
        #endregion

        #region Filter
        public async Task<List<CustomerDto>> GetByTypeAsync(CustomerType type)
        {
            try
            {
                var customers = await _repository.GetByTypeAsync(type);
                return _mapper.Map<List<CustomerDto>>(customers);
            }
            catch (NpgsqlException ex)
            {
                _logger.LogError(ex, "Database connection error in {Operation} Customer: Type={CustomerType}",
                    nameof(GetByTypeAsync), type);
                throw new BusinessRuleException("Database is unavailable");
            }
        }
        #endregion
    }
}