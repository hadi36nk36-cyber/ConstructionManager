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
    public class EquipmentServiceTests
    {
        private readonly Mock<IEquipmentRepository> _repositoryMock;
        private readonly Mock<IMapper> _mapperMock;
        private readonly Mock<ILogger<EquipmentService>> _loggerMock;
        private readonly EquipmentService _service;

        public EquipmentServiceTests()
        {
            _repositoryMock = new Mock<IEquipmentRepository>();
            _mapperMock = new Mock<IMapper>();
            _loggerMock = new Mock<ILogger<EquipmentService>>();
            _service = new EquipmentService(_repositoryMock.Object, _mapperMock.Object, _loggerMock.Object);
        }

        [Fact]
        public async Task AddAsync_WhenCodeDuplicate_Throws()
        {
            var dto = new EquipmentDto { Code = "E1" };
            _repositoryMock.Setup(r => r.IsCodeDuplicateAsync("E1")).ReturnsAsync(true);
            await Assert.ThrowsAsync<DuplicateException>(() => _service.AddAsync(dto));
        }

        [Fact]
        public async Task AddAsync_WhenSerialNumberDuplicate_Throws()
        {
            var dto = new EquipmentDto { Code = "E1", SerialNumber = "SN1" };
            _repositoryMock.Setup(r => r.IsCodeDuplicateAsync("E1")).ReturnsAsync(false);
            _repositoryMock.Setup(r => r.IsSerialNumberDuplicateAsync("SN1")).ReturnsAsync(true);
            await Assert.ThrowsAsync<DuplicateException>(() => _service.AddAsync(dto));
        }

        [Fact]
        public async Task DeleteAsync_WhenHasPayments_Throws()
        {
            _repositoryMock.Setup(r => r.HasPaymentsAsync(1)).ReturnsAsync(true);
            await Assert.ThrowsAsync<BusinessRuleException>(() => _service.DeleteAsync(1));
        }
    }
}