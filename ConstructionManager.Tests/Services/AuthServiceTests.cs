using AutoMapper;
using ConstructionManager.Api.Application.DTOs;
using ConstructionManager.Api.Application.Interfaces.Repositories;
using ConstructionManager.Api.Application.Services;
using ConstructionManager.Api.Domain.Entities;
using ConstructionManager.Api.Domain.Exceptions;
using ConstructionManager.Api.Infrastructure.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace ConstructionManager.Tests.Services
{
    public class AuthServiceTests
    {
        private readonly Mock<IUserRepository> _repositoryMock;
        private readonly Mock<ILogger<AuthService>> _loggerMock;
        private readonly Mock<IConfiguration> _configurationMock;
        private readonly JwtTokenService _jwtTokenService;
        private readonly PasswordHasher _passwordHasher;
        private readonly AuthService _service;

        public AuthServiceTests()
        {
            _repositoryMock = new Mock<IUserRepository>();
            _loggerMock = new Mock<ILogger<AuthService>>();

            _configurationMock = new Mock<IConfiguration>();
            _configurationMock.Setup(c => c["Jwt:Key"]).Returns("this_is_a_super_secret_key_for_testing_12345");
            _configurationMock.Setup(c => c["Jwt:Issuer"]).Returns("TestIssuer");
            _configurationMock.Setup(c => c["Jwt:Audience"]).Returns("TestAudience");
            _configurationMock.Setup(c => c["Jwt:AccessTokenExpirationMinutes"]).Returns("15");
            _configurationMock.Setup(c => c["Jwt:RefreshTokenExpirationDays"]).Returns("7");

            _jwtTokenService = new JwtTokenService(_configurationMock.Object);
            _passwordHasher = new PasswordHasher();

            _service = new AuthService(
                _repositoryMock.Object,
                _jwtTokenService,
                _passwordHasher,
                _configurationMock.Object,
                _loggerMock.Object
            );
        }

        [Fact]
        public async Task LoginAsync_WhenUserNotFound_ThrowsNotFoundException()
        {
            var dto = new LoginDto { Username = "notfound", Password = "123" };
            _repositoryMock.Setup(r => r.SearchByUsernameAsync("notfound")).ReturnsAsync((User)null);

            await Assert.ThrowsAsync<NotFoundException>(() => _service.LoginAsync(dto));
        }

        [Fact]
        public async Task LoginAsync_WhenUserInactive_ThrowsNotFoundException()
        {
            var dto = new LoginDto { Username = "inactive", Password = "123" };
            var user = new User { Username = "inactive", IsActive = false };
            _repositoryMock.Setup(r => r.SearchByUsernameAsync("inactive")).ReturnsAsync(user);

            await Assert.ThrowsAsync<NotFoundException>(() => _service.LoginAsync(dto));
        }

        [Fact]
        public async Task LoginAsync_WhenPasswordIsWrong_ThrowsValidationException()
        {
            var dto = new LoginDto { Username = "admin", Password = "wrong" };
            var user = new User { Username = "admin", IsActive = true, PasswordHash = _passwordHasher.Hash("123") };
            _repositoryMock.Setup(r => r.SearchByUsernameAsync("admin")).ReturnsAsync(user);

            await Assert.ThrowsAsync<ValidationException>(() => _service.LoginAsync(dto));
        }

        [Fact]
        public async Task LoginAsync_WhenValid_ReturnsTokens()
        {
            var dto = new LoginDto { Username = "admin", Password = "123" };
            var user = new User { Id = 1, Username = "admin", Role = "Admin", IsActive = true, PasswordHash = _passwordHasher.Hash("123") };
            _repositoryMock.Setup(r => r.SearchByUsernameAsync("admin")).ReturnsAsync(user);

            var result = await _service.LoginAsync(dto);

            Assert.NotNull(result);
            Assert.NotNull(result.AccessToken);
            Assert.NotNull(result.RefreshToken);
            _repositoryMock.Verify(r => r.UpdateAsync(It.IsAny<User>()), Times.Once); // چون باید توکن رو ذخیره کنه
        }

        [Fact]
        public async Task RegisterAsync_WhenUsernameDuplicate_ThrowsDuplicateException()
        {
            var dto = new RegisterDto { Username = "admin", Password = "123" };
            _repositoryMock.Setup(r => r.IsUsernameDuplicateAsync("admin")).ReturnsAsync(true);

            await Assert.ThrowsAsync<DuplicateException>(() => _service.RegisterAsync(dto));
        }

        [Fact]
        public async Task RegisterAsync_WhenValid_ReturnsTokens()
        {
            var dto = new RegisterDto { Username = "newuser", Password = "123" };
            _repositoryMock.Setup(r => r.IsUsernameDuplicateAsync("newuser")).ReturnsAsync(false);

            var result = await _service.RegisterAsync(dto);

            Assert.NotNull(result);
            Assert.NotNull(result.AccessToken);
            _repositoryMock.Verify(r => r.AddAsync(It.IsAny<User>()), Times.Once);
        }
        [Fact]
        public async Task RefreshTokenAsync_WhenTokenIsInvalid_ThrowsValidationException()
        {
            var dto = new RefreshTokenDto { RefreshToken = "invalid-token" };
            _repositoryMock.Setup(r => r.GetByRefreshTokenAsync("invalid-token")).ReturnsAsync((User)null);

            await Assert.ThrowsAsync<ValidationException>(() => _service.RefreshTokenAsync(dto));
        }

        [Fact]
        public async Task RefreshTokenAsync_WhenTokenIsExpired_ThrowsValidationException()
        {
            var dto = new RefreshTokenDto { RefreshToken = "expired-token" };
            var user = new User
            {
                Username = "admin",
                RefreshToken = "expired-token",
                RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(-1) 
            };
            _repositoryMock.Setup(r => r.GetByRefreshTokenAsync("expired-token")).ReturnsAsync(user);

            await Assert.ThrowsAsync<ValidationException>(() => _service.RefreshTokenAsync(dto));
        }

        [Fact]
        public async Task RefreshTokenAsync_WhenTokenIsValid_ReturnsNewTokens()
        {
            var dto = new RefreshTokenDto { RefreshToken = "valid-token" };
            var user = new User
            {
                Id = 1,
                Username = "admin",
                Role = "Admin",
                RefreshToken = "valid-token",
                RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(1) 
            };
            _repositoryMock.Setup(r => r.GetByRefreshTokenAsync("valid-token")).ReturnsAsync(user);

            var result = await _service.RefreshTokenAsync(dto);

            Assert.NotNull(result);
            Assert.NotNull(result.AccessToken);
            Assert.NotEqual("valid-token", result.RefreshToken); 
            _repositoryMock.Verify(r => r.UpdateAsync(It.IsAny<User>()), Times.Once);
        }

        [Fact]
        public async Task LogoutAsync_WhenUserExists_ClearsRefreshToken()
        {
            var user = new User
            {
                Username = "admin",
                RefreshToken = "some-token",
                RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(1)
            };
            _repositoryMock.Setup(r => r.SearchByUsernameAsync("admin")).ReturnsAsync(user);

            await _service.LogoutAsync("admin");

            Assert.Null(user.RefreshToken);
            Assert.Null(user.RefreshTokenExpiryTime);
            _repositoryMock.Verify(r => r.UpdateAsync(user), Times.Once);
        }

        [Fact]
        public async Task LogoutAsync_WhenUserNotFound_DoesNothing()
        {
            _repositoryMock.Setup(r => r.SearchByUsernameAsync("notfound")).ReturnsAsync((User)null);

            await _service.LogoutAsync("notfound");

            _repositoryMock.Verify(r => r.UpdateAsync(It.IsAny<User>()), Times.Never);
        }
    }
}