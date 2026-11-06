using AutoMapper;
using ConstructionManager.Api.Application.DTOs;
using ConstructionManager.Api.Application.Interfaces.Repositories;
using ConstructionManager.Api.Application.Services;
using ConstructionManager.Api.Domain.Entities;
using ConstructionManager.Api.Domain.Exceptions;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace ConstructionManager.Tests.Services
{
    public class EmployeeServiceTests
    {
        private readonly Mock<IEmployeeRepository> _repositoryMock;
        private readonly Mock<IMapper> _mapperMock;
        private readonly Mock<ILogger<EmployeeService>> _loggerMock;
        private readonly EmployeeService _service;

        public EmployeeServiceTests()
        {
            _repositoryMock = new Mock<IEmployeeRepository>();
            _mapperMock = new Mock<IMapper>();
            _loggerMock = new Mock<ILogger<EmployeeService>>();
            _service = new EmployeeService(_repositoryMock.Object, _mapperMock.Object, _loggerMock.Object);
        }

        [Fact]
        public async Task AddAsync_WhenNationalCodeDuplicate_Throws()
        {
            var dto = new EmployeeDto { NationalCode = "123" };
            _repositoryMock.Setup(r => r.IsNationalCodeDuplicateAsync("123")).ReturnsAsync(true);
            await Assert.ThrowsAsync<DuplicateException>(() => _service.AddAsync(dto));
        }

        [Fact]
        public async Task AddAsync_WhenEmployeeCodeDuplicate_Throws()
        {
            var dto = new EmployeeDto { NationalCode = "123", EmployeeCode = "E1", Phone1 = "0912" };
            _repositoryMock.Setup(r => r.IsNationalCodeDuplicateAsync("123")).ReturnsAsync(false);
            _repositoryMock.Setup(r => r.IsEmployeeCodeDuplicateAsync("E1")).ReturnsAsync(true);
            await Assert.ThrowsAsync<DuplicateException>(() => _service.AddAsync(dto));
        }

        [Fact]
        public async Task AddAsync_WhenPhoneDuplicate_Throws()
        {
            var dto = new EmployeeDto { NationalCode = "123", EmployeeCode = "E1", Phone1 = "0912" };
            _repositoryMock.Setup(r => r.IsNationalCodeDuplicateAsync(It.IsAny<string>())).ReturnsAsync(false);
            _repositoryMock.Setup(r => r.IsEmployeeCodeDuplicateAsync(It.IsAny<string>())).ReturnsAsync(false);
            _repositoryMock.Setup(r => r.IsPhoneDuplicateAsync("0912")).ReturnsAsync(true);
            await Assert.ThrowsAsync<DuplicateException>(() => _service.AddAsync(dto));
        }

        [Fact]
        public async Task DeleteAsync_WhenHasProjectAssignments_Throws()
        {
            _repositoryMock.Setup(r => r.HasProjectAssignmentsAsync(1)).ReturnsAsync(true);
            await Assert.ThrowsAsync<BusinessRuleException>(() => _service.DeleteAsync(1));
        }

        [Fact]
        public async Task DeleteAsync_WhenHasPayments_Throws()
        {
            _repositoryMock.Setup(r => r.HasProjectAssignmentsAsync(1)).ReturnsAsync(false);
            _repositoryMock.Setup(r => r.HasPaymentsAsync(1)).ReturnsAsync(true);
            await Assert.ThrowsAsync<BusinessRuleException>(() => _service.DeleteAsync(1));
        }
    }
}