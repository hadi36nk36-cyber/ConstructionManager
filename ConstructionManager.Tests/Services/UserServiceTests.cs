using AutoMapper;
using ConstructionManager.Api.Application.DTOs;
using ConstructionManager.Api.Application.Interfaces.Repositories;
using ConstructionManager.Api.Application.Services;
using ConstructionManager.Api.Domain.Exceptions;
using ConstructionManager.Api.Infrastructure.Services;
using ConstructionManager.Api.Domain.Entities;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace ConstructionManager.Tests.Services
{
    public class UserServiceTests
    {
        private readonly Mock<IUserRepository> _repositoryMock;
        private readonly Mock<IMapper> _mapperMock;
        private readonly Mock<ILogger<UserService>> _loggerMock;
        private readonly PasswordHasher _passwordHasher;
        private readonly UserService _service;

        public UserServiceTests()
        {
            _repositoryMock = new Mock<IUserRepository>();
            _mapperMock = new Mock<IMapper>();
            _loggerMock = new Mock<ILogger<UserService>>();

            _passwordHasher = new PasswordHasher();

            _service = new UserService(_repositoryMock.Object, _mapperMock.Object, _passwordHasher, _loggerMock.Object);
        }

        [Fact]
        public async Task AddAsync_WhenUsernameDuplicate_Throws()
        {
            var dto = new UserDto { Username = "admin", Password = "123" };
            _repositoryMock.Setup(r => r.IsUsernameDuplicateAsync("admin")).ReturnsAsync(true);
            await Assert.ThrowsAsync<DuplicateException>(() => _service.AddAsync(dto));
        }

        [Fact]
        public async Task AddAsync_WhenValid_AddsUser()
        {
            var dto = new UserDto { Username = "user1", Password = "123", Role = "User" };
            _repositoryMock.Setup(r => r.IsUsernameDuplicateAsync(It.IsAny<string>())).ReturnsAsync(false);

            await _service.AddAsync(dto);

            _repositoryMock.Verify(r => r.AddAsync(It.IsAny<User>()), Times.Once);
        }
    }
}