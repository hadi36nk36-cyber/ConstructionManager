using ConstructionManager.Api.Application.DTOs;
using ConstructionManager.Api.Application.Interfaces.Repositories;
using ConstructionManager.Api.Application.Interfaces.Services;
using ConstructionManager.Api.Domain.Entities;
using ConstructionManager.Api.Domain.Exceptions;
using ConstructionManager.Api.Infrastructure.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace ConstructionManager.Api.Application.Services
{
    public class AuthService : IAuthService
    {
        private readonly IUserRepository _userRepository;
        private readonly JwtTokenService _jwtTokenService;
        private readonly PasswordHasher _passwordHasher;
        private readonly IConfiguration _configuration;
        private readonly ILogger<AuthService> _logger;

        public AuthService(
            IUserRepository userRepository,
            JwtTokenService jwtTokenService,
            PasswordHasher passwordHasher,
            IConfiguration configuration,
            ILogger<AuthService> logger)
        {
            _userRepository = userRepository;
            _jwtTokenService = jwtTokenService;
            _passwordHasher = passwordHasher;
            _configuration = configuration;
            _logger = logger;
        }

        public async Task<TokenDto> LoginAsync(LoginDto dto)
        {
            var user = await _userRepository.SearchByUsernameAsync(dto.Username);

            if (user == null || !user.IsActive)
            {
                _logger.LogWarning("Login failed: username not found or inactive — {Username}", dto.Username);
                throw new NotFoundException("Username or password is incorrect");
            }

            if (!_passwordHasher.Verify(dto.Password, user.PasswordHash))
            {
                _logger.LogWarning("Login failed: invalid password — {Username}", dto.Username);
                throw new ValidationException("Username or password is incorrect");
            }

            var accessToken = _jwtTokenService.GenerateAccessToken(user);
            var refreshToken = _jwtTokenService.GenerateRefreshToken();

            user.RefreshToken = refreshToken;
            user.RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(
                int.Parse(_configuration["Jwt:RefreshTokenExpirationDays"] ?? "7"));
            user.LastLoginAt = DateTime.UtcNow;

            await _userRepository.UpdateAsync(user);

            return new TokenDto
            {
                AccessToken = accessToken,
                RefreshToken = refreshToken,
                AccessTokenExpiry = DateTime.UtcNow.AddMinutes(
                    int.Parse(_configuration["Jwt:AccessTokenExpirationMinutes"] ?? "15"))
            };
        }

        public async Task<TokenDto> RegisterAsync(RegisterDto dto, string role = "User")
        {
            if (await _userRepository.IsUsernameDuplicateAsync(dto.Username))
            {
                _logger.LogWarning("Register failed: username duplicate — {Username}", dto.Username);
                throw new DuplicateException("Username is duplicate");
            }

            var refreshToken = _jwtTokenService.GenerateRefreshToken();

            var user = new User
            {
                Username = dto.Username,
                PasswordHash = _passwordHasher.Hash(dto.Password),
                Role = "User",
                IsActive = true,
                CreatedAt = DateTime.UtcNow,
                RefreshToken = refreshToken,
                RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(
                    int.Parse(_configuration["Jwt:RefreshTokenExpirationDays"] ?? "7"))
            };

            await _userRepository.AddAsync(user);

            return new TokenDto
            {
                AccessToken = _jwtTokenService.GenerateAccessToken(user),
                RefreshToken = refreshToken,
                AccessTokenExpiry = DateTime.UtcNow.AddMinutes(
                    int.Parse(_configuration["Jwt:AccessTokenExpirationMinutes"] ?? "15"))
            };
        }

        public async Task<TokenDto> RefreshTokenAsync(RefreshTokenDto dto)
        {
            var user = await _userRepository.GetByRefreshTokenAsync(dto.RefreshToken);

            if (user == null || user.RefreshTokenExpiryTime <= DateTime.UtcNow)
                throw new ValidationException("Invalid or expired refresh token");

            var newRefreshToken = _jwtTokenService.GenerateRefreshToken();
            user.RefreshToken = newRefreshToken;
            user.RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(
                int.Parse(_configuration["Jwt:RefreshTokenExpirationDays"] ?? "7"));

            await _userRepository.UpdateAsync(user);

            return new TokenDto
            {
                AccessToken = _jwtTokenService.GenerateAccessToken(user),
                RefreshToken = newRefreshToken,
                AccessTokenExpiry = DateTime.UtcNow.AddMinutes(
                    int.Parse(_configuration["Jwt:AccessTokenExpirationMinutes"] ?? "15"))
            };
        }

        public async Task LogoutAsync(string username)
        {
            var user = await _userRepository.SearchByUsernameAsync(username);
            if (user != null)
            {
                user.RefreshToken = null;
                user.RefreshTokenExpiryTime = null;
                await _userRepository.UpdateAsync(user);
            }
        }
    }
}