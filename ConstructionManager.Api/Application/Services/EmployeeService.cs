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
    public class EmployeeService : IEmployeeService
    {
        private readonly IEmployeeRepository _repository;
        private readonly IMapper _mapper;
        private readonly ILogger<EmployeeService> _logger;

        public EmployeeService(IEmployeeRepository repository, IMapper mapper, ILogger<EmployeeService> logger)
        {
            _repository = repository;
            _mapper = mapper;
            _logger = logger;
        }

        #region CRUD
        public async Task<List<EmployeeDto>> GetAllAsync()
        {
            try
            {
                var employees = await _repository.GetAllAsync();
                return _mapper.Map<List<EmployeeDto>>(employees);
            }
            catch (SqlException ex)
            {
                _logger.LogError(ex, "Database connection error in {Operation} Employee", nameof(GetAllAsync));
                throw new BusinessRuleException("Database is unavailable");
            }
        }

        public async Task AddAsync(EmployeeDto dto)
        {
            try
            {
                if (await _repository.IsNationalCodeDuplicateAsync(dto.NationalCode))
                    throw new DuplicateException("Employee national code is duplicate");

                if (await _repository.IsEmployeeCodeDuplicateAsync(dto.EmployeeCode))
                    throw new DuplicateException("Employee code is duplicate");

                if (await _repository.IsPhoneDuplicateAsync(dto.Phone1))
                    throw new DuplicateException("Employee phone is duplicate");

                var employee = _mapper.Map<Employee>(dto);
                await _repository.AddAsync(employee);
            }
            catch (DuplicateException ex)
            {
                _logger.LogWarning(ex, "Duplicate employee in {Operation}: NationalCode={NationalCode}, EmployeeCode={EmployeeCode}",
                    nameof(AddAsync), dto.NationalCode, dto.EmployeeCode);
                throw;
            }
            catch (SqlException ex)
            {
                _logger.LogError(ex, "Database error in {Operation} Employee: NationalCode={NationalCode}", nameof(AddAsync), dto.NationalCode);
                throw new BusinessRuleException("Database is unavailable");
            }
            catch (DbUpdateException ex)
            {
                _logger.LogError(ex, "Database update error in {Operation} Employee: NationalCode={NationalCode}", nameof(AddAsync), dto.NationalCode);
                throw new BusinessRuleException("Database operation failed");
            }
        }

        public async Task UpdateAsync(EmployeeDto dto)
        {
            try
            {
                var existingByNational = await _repository.SearchByNationalCodeAsync(dto.NationalCode);
                if (existingByNational != null && existingByNational.Id != dto.Id)
                    throw new DuplicateException("Employee national code is duplicate");

                var existingByCode = await _repository.SearchByEmployeeCodeAsync(dto.EmployeeCode);
                if (existingByCode != null && existingByCode.Id != dto.Id)
                    throw new DuplicateException("Employee code is duplicate");

                var employee = _mapper.Map<Employee>(dto);
                await _repository.UpdateAsync(employee);
            }
            catch (DuplicateException ex)
            {
                _logger.LogWarning(ex, "Duplicate employee in {Operation}: NationalCode={NationalCode}, Id={Id}",
                    nameof(UpdateAsync), dto.NationalCode, dto.Id);
                throw;
            }
            catch (NotFoundException ex)
            {
                _logger.LogWarning(ex, "Employee not found in {Operation}: Id={Id}", nameof(UpdateAsync), dto.Id);
                throw;
            }
            catch (SqlException ex)
            {
                _logger.LogError(ex, "Database error in {Operation} Employee: Id={Id}", nameof(UpdateAsync), dto.Id);
                throw new BusinessRuleException("Database is unavailable");
            }
            catch (DbUpdateException ex)
            {
                _logger.LogError(ex, "Database update error in {Operation} Employee: Id={Id}", nameof(UpdateAsync), dto.Id);
                throw new BusinessRuleException("Database operation failed");
            }
        }

        public async Task DeleteAsync(int id)
        {
            try
            {
                if (await _repository.HasProjectAssignmentsAsync(id))
                    throw new BusinessRuleException("Employee has project assignments");

                if (await _repository.HasPaymentsAsync(id))
                    throw new BusinessRuleException("Employee has payments");

                await _repository.DeleteAsync(id);
            }
            catch (BusinessRuleException ex)
            {
                _logger.LogWarning(ex, "Business rule violation in {Operation} Employee: Id={Id}", nameof(DeleteAsync), id);
                throw;
            }
            catch (NotFoundException ex)
            {
                _logger.LogWarning(ex, "Employee not found in {Operation}: Id={Id}", nameof(DeleteAsync), id);
                throw;
            }
            catch (SqlException ex)
            {
                _logger.LogError(ex, "Database error in {Operation} Employee: Id={Id}", nameof(DeleteAsync), id);
                throw new BusinessRuleException("Database is unavailable");
            }
            catch (DbUpdateException ex)
            {
                _logger.LogError(ex, "Database update error in {Operation} Employee: Id={Id}", nameof(DeleteAsync), id);
                throw new BusinessRuleException("Database operation failed");
            }
        }
        #endregion

        #region Search
        public async Task<List<EmployeeDto>> SearchByNameAsync(string name)
        {
            try
            {
                var employees = await _repository.SearchByNameAsync(name);
                return _mapper.Map<List<EmployeeDto>>(employees);
            }
            catch (SqlException ex)
            {
                _logger.LogError(ex, "Database connection error in {Operation} Employee: Name={Name}", nameof(SearchByNameAsync), name);
                throw new BusinessRuleException("Database is unavailable");
            }
        }

        public async Task<EmployeeDto?> SearchByNationalCodeAsync(string nationalCode)
        {
            try
            {
                var employee = await _repository.SearchByNationalCodeAsync(nationalCode);
                return _mapper.Map<EmployeeDto>(employee);
            }
            catch (SqlException ex)
            {
                _logger.LogError(ex, "Database connection error in {Operation} Employee: NationalCode={NationalCode}", nameof(SearchByNationalCodeAsync), nationalCode);
                throw new BusinessRuleException("Database is unavailable");
            }
        }

        public async Task<EmployeeDto?> SearchByEmployeeCodeAsync(string employeeCode)
        {
            try
            {
                var employee = await _repository.SearchByEmployeeCodeAsync(employeeCode);
                return _mapper.Map<EmployeeDto>(employee);
            }
            catch (SqlException ex)
            {
                _logger.LogError(ex, "Database connection error in {Operation} Employee: EmployeeCode={EmployeeCode}", nameof(SearchByEmployeeCodeAsync), employeeCode);
                throw new BusinessRuleException("Database is unavailable");
            }
        }
        #endregion

        #region Filter
        public async Task<List<EmployeeDto>> GetByStatusAsync(EmployeeStatus status)
        {
            try
            {
                var employees = await _repository.GetByStatusAsync(status);
                return _mapper.Map<List<EmployeeDto>>(employees);
            }
            catch (SqlException ex)
            {
                _logger.LogError(ex, "Database connection error in {Operation} Employee: Status={Status}", nameof(GetByStatusAsync), status);
                throw new BusinessRuleException("Database is unavailable");
            }
        }

        public async Task<List<EmployeeDto>> GetBySpecialityAsync(string speciality)
        {
            try
            {
                var employees = await _repository.GetBySpecialityAsync(speciality);
                return _mapper.Map<List<EmployeeDto>>(employees);
            }
            catch (SqlException ex)
            {
                _logger.LogError(ex, "Database connection error in {Operation} Employee: Speciality={Speciality}", nameof(GetBySpecialityAsync), speciality);
                throw new BusinessRuleException("Database is unavailable");
            }
        }
        #endregion
    }
}