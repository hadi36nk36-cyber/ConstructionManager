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
    public class EquipmentService : IEquipmentService
    {
        private readonly IEquipmentRepository _repository;
        private readonly IMapper _mapper;
        private readonly ILogger<EquipmentService> _logger;

        public EquipmentService(IEquipmentRepository repository, IMapper mapper, ILogger<EquipmentService> logger)
        {
            _repository = repository;
            _mapper = mapper;
            _logger = logger;
        }

        #region CRUD
        public async Task<List<EquipmentDto>> GetAllAsync()
        {
            try
            {
                var equipments = await _repository.GetAllAsync();
                return _mapper.Map<List<EquipmentDto>>(equipments);
            }
            catch (SqlException ex)
            {
                _logger.LogError(ex, "Database connection error in {Operation} Equipment", nameof(GetAllAsync));
                throw new BusinessRuleException("Database is unavailable");
            }
        }

        public async Task AddAsync(EquipmentDto dto)
        {
            try
            {
                if (await _repository.IsCodeDuplicateAsync(dto.Code))
                    throw new DuplicateException("Equipment code is duplicate");

                if (await _repository.IsSerialNumberDuplicateAsync(dto.SerialNumber))
                    throw new DuplicateException("Equipment serial number is duplicate");

                var equipment = _mapper.Map<Equipment>(dto);
                await _repository.AddAsync(equipment);
            }
            catch (DuplicateException ex)
            {
                _logger.LogWarning(ex, "Duplicate equipment in {Operation}: Code={Code}, SerialNumber={SerialNumber}",
                    nameof(AddAsync), dto.Code, dto.SerialNumber);
                throw;
            }
            catch (SqlException ex)
            {
                _logger.LogError(ex, "Database error in {Operation} Equipment: Code={Code}", nameof(AddAsync), dto.Code);
                throw new BusinessRuleException("Database is unavailable");
            }
            catch (DbUpdateException ex)
            {
                _logger.LogError(ex, "Database update error in {Operation} Equipment: Code={Code}", nameof(AddAsync), dto.Code);
                throw new BusinessRuleException("Database operation failed");
            }
        }

        public async Task UpdateAsync(EquipmentDto dto)
        {
            try
            {
                var existing = await _repository.SearchByCodeAsync(dto.Code);
                if (existing != null && existing.Id != dto.Id)
                    throw new DuplicateException("Equipment code is duplicate");

                var existingSerial = await _repository.SearchBySerialNumberAsync(dto.SerialNumber);
                if (existingSerial != null && existingSerial.Id != dto.Id)
                    throw new DuplicateException("Equipment serial number is duplicate");

                var equipment = _mapper.Map<Equipment>(dto);
                await _repository.UpdateAsync(equipment);
            }
            catch (DuplicateException ex)
            {
                _logger.LogWarning(ex, "Duplicate equipment in {Operation}: Code={Code}, Id={Id}", nameof(UpdateAsync), dto.Code, dto.Id);
                throw;
            }
            catch (NotFoundException ex)
            {
                _logger.LogWarning(ex, "Equipment not found in {Operation}: Id={Id}", nameof(UpdateAsync), dto.Id);
                throw;
            }
            catch (SqlException ex)
            {
                _logger.LogError(ex, "Database error in {Operation} Equipment: Id={Id}", nameof(UpdateAsync), dto.Id);
                throw new BusinessRuleException("Database is unavailable");
            }
            catch (DbUpdateException ex)
            {
                _logger.LogError(ex, "Database update error in {Operation} Equipment: Id={Id}", nameof(UpdateAsync), dto.Id);
                throw new BusinessRuleException("Database operation failed");
            }
        }

        public async Task DeleteAsync(int id)
        {
            try
            {
                if (await _repository.HasPaymentsAsync(id))
                    throw new BusinessRuleException("Equipment has payments");

                await _repository.DeleteAsync(id);
            }
            catch (BusinessRuleException ex)
            {
                _logger.LogWarning(ex, "Business rule violation in {Operation} Equipment: Id={Id}", nameof(DeleteAsync), id);
                throw;
            }
            catch (NotFoundException ex)
            {
                _logger.LogWarning(ex, "Equipment not found in {Operation}: Id={Id}", nameof(DeleteAsync), id);
                throw;
            }
            catch (SqlException ex)
            {
                _logger.LogError(ex, "Database error in {Operation} Equipment: Id={Id}", nameof(DeleteAsync), id);
                throw new BusinessRuleException("Database is unavailable");
            }
            catch (DbUpdateException ex)
            {
                _logger.LogError(ex, "Database update error in {Operation} Equipment: Id={Id}", nameof(DeleteAsync), id);
                throw new BusinessRuleException("Database operation failed");
            }
        }
        #endregion

        #region Search
        public async Task<EquipmentDto?> SearchByCodeAsync(string code)
        {
            try
            {
                var equipment = await _repository.SearchByCodeAsync(code);
                return _mapper.Map<EquipmentDto>(equipment);
            }
            catch (SqlException ex)
            {
                _logger.LogError(ex, "Database connection error in {Operation} Equipment: Code={Code}", nameof(SearchByCodeAsync), code);
                throw new BusinessRuleException("Database is unavailable");
            }
        }

        public async Task<EquipmentDto?> SearchBySerialNumberAsync(string serialNumber)
        {
            try
            {
                var equipment = await _repository.SearchBySerialNumberAsync(serialNumber);
                return _mapper.Map<EquipmentDto>(equipment);
            }
            catch (SqlException ex)
            {
                _logger.LogError(ex, "Database connection error in {Operation} Equipment: SerialNumber={SerialNumber}", nameof(SearchBySerialNumberAsync), serialNumber);
                throw new BusinessRuleException("Database is unavailable");
            }
        }
        #endregion

        #region Filter
        public async Task<List<EquipmentDto>> GetByTypeAsync(EquipmentType type)
        {
            try
            {
                var equipments = await _repository.GetByTypeAsync(type);
                return _mapper.Map<List<EquipmentDto>>(equipments);
            }
            catch (SqlException ex)
            {
                _logger.LogError(ex, "Database connection error in {Operation} Equipment: Type={Type}", nameof(GetByTypeAsync), type);
                throw new BusinessRuleException("Database is unavailable");
            }
        }

        public async Task<List<EquipmentDto>> GetByStatusAsync(EquipmentStatus status)
        {
            try
            {
                var equipments = await _repository.GetByStatusAsync(status);
                return _mapper.Map<List<EquipmentDto>>(equipments);
            }
            catch (SqlException ex)
            {
                _logger.LogError(ex, "Database connection error in {Operation} Equipment: Status={Status}", nameof(GetByStatusAsync), status);
                throw new BusinessRuleException("Database is unavailable");
            }
        }
        #endregion
    }
}