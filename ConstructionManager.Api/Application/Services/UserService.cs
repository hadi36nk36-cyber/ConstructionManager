using AutoMapper;
using ConstructionManager.Api.Application.DTOs;
using ConstructionManager.Api.Application.Interfaces.Repositories;
using ConstructionManager.Api.Application.Interfaces.Services;
using ConstructionManager.Api.Domain.Entities;
using ConstructionManager.Api.Domain.Exceptions;
using ConstructionManager.Api.Infrastructure.Services;
using Microsoft.Extensions.Logging;
using Npgsql;
using Microsoft.EntityFrameworkCore;

namespace ConstructionManager.Api.Application.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _repository;
        private readonly IMapper _mapper;
        private readonly PasswordHasher _passwordHasher;
        private readonly ILogger<UserService> _logger;

        public UserService(
            IUserRepository repository,
            IMapper mapper,
            PasswordHasher passwordHasher,
            ILogger<UserService> logger)
        {
            _repository = repository;
            _mapper = mapper;
            _passwordHasher = passwordHasher;
            _logger = logger;
        }

        #region CRUD
        public async Task<List<UserDto>> GetAllAsync()
        {
            try
            {
                var users = await _repository.GetAllAsync();
                return _mapper.Map<List<UserDto>>(users);
            }
            catch (NpgsqlException ex)
            {
                _logger.LogError(ex, "Database connection error in {Operation} User", nameof(GetAllAsync));
                throw new BusinessRuleException("Database is unavailable");
            }
        }

        public async Task AddAsync(UserDto dto)
        {
            try
            {
                if (await _repository.IsUsernameDuplicateAsync(dto.Username))
                    throw new DuplicateException("Username is duplicate");

                var user = new User
                {
                    Username = dto.Username,
                    PasswordHash = _passwordHasher.Hash(dto.Password),
                    Role = dto.Role,
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow
                };

                await _repository.AddAsync(user);
            }
            catch (DuplicateException ex)
            {
                _logger.LogWarning(ex, "Duplicate user in {Operation}: Username={Username}", nameof(AddAsync), dto.Username);
                throw;
            }
            catch (NpgsqlException ex)
            {
                _logger.LogError(ex, "Database error in {Operation} User: Username={Username}", nameof(AddAsync), dto.Username);
                throw new BusinessRuleException("Database is unavailable");
            }
            catch (DbUpdateException ex)
            {
                _logger.LogError(ex, "Database update error in {Operation} User: Username={Username}", nameof(AddAsync), dto.Username);
                throw new BusinessRuleException("Database operation failed");
            }
        }

        public async Task UpdateAsync(UserDto dto)
        {
            try
            {
                var existing = await _repository.GetByIdAsync(dto.Id);
                if (existing == null)
                    throw new NotFoundException("User not found");

                existing.Role = dto.Role;   // فقط Role

                await _repository.UpdateAsync(existing);
            }
            catch (NotFoundException ex)
            {
                _logger.LogWarning(ex, "User not found in {Operation}: Id={Id}", nameof(UpdateAsync), dto.Id);
                throw;
            }
            catch (NpgsqlException ex)
            {
                _logger.LogError(ex, "Database error in {Operation} User: Id={Id}", nameof(UpdateAsync), dto.Id);
                throw new BusinessRuleException("Database is unavailable");
            }
            catch (DbUpdateException ex)
            {
                _logger.LogError(ex, "Database update error in {Operation} User: Id={Id}", nameof(UpdateAsync), dto.Id);
                throw new BusinessRuleException("Database operation failed");
            }
        }

        public async Task DeleteAsync(int id)
        {
            try
            {
                await _repository.DeleteAsync(id);   // Soft Delete
            }
            catch (NotFoundException ex)
            {
                _logger.LogWarning(ex, "User not found in {Operation}: Id={Id}", nameof(DeleteAsync), id);
                throw;
            }
            catch (NpgsqlException ex)
            {
                _logger.LogError(ex, "Database error in {Operation} User: Id={Id}", nameof(DeleteAsync), id);
                throw new BusinessRuleException("Database is unavailable");
            }
            catch (DbUpdateException ex)
            {
                _logger.LogError(ex, "Database update error in {Operation} User: Id={Id}", nameof(DeleteAsync), id);
                throw new BusinessRuleException("Database operation failed");
            }
        }
        #endregion

        #region Search
        public async Task<UserDto?> SearchByUsernameAsync(string username)
        {
            try
            {
                var user = await _repository.SearchByUsernameAsync(username);
                return _mapper.Map<UserDto>(user);
            }
            catch (NpgsqlException ex)
            {
                _logger.LogError(ex, "Database connection error in {Operation} User: Username={Username}", nameof(SearchByUsernameAsync), username);
                throw new BusinessRuleException("Database is unavailable");
            }
        }
        #endregion

        #region Filter
        public async Task<List<UserDto>> GetByRoleAsync(string role)
        {
            try
            {
                var users = await _repository.GetByRoleAsync(role);
                return _mapper.Map<List<UserDto>>(users);
            }
            catch (NpgsqlException ex)
            {
                _logger.LogError(ex, "Database connection error in {Operation} User: Role={Role}", nameof(GetByRoleAsync), role);
                throw new BusinessRuleException("Database is unavailable");
            }
        }
        #endregion
    }
}