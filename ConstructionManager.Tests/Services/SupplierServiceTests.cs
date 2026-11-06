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
    public class SupplierServiceTests
    {
        private readonly Mock<ISupplierRepository> _repositoryMock;
        private readonly Mock<IMapper> _mapperMock;
        private readonly Mock<ILogger<SupplierService>> _loggerMock;
        private readonly SupplierService _service;

        public SupplierServiceTests()
        {
            _repositoryMock = new Mock<ISupplierRepository>();
            _mapperMock = new Mock<IMapper>();
            _loggerMock = new Mock<ILogger<SupplierService>>();
            _service = new SupplierService(_repositoryMock.Object, _mapperMock.Object, _loggerMock.Object);
        }

        [Fact]
        public async Task AddAsync_WhenCodeDuplicate_Throws()
        {
            var dto = new SupplierDto { Code = "S1" };
            _repositoryMock.Setup(r => r.IsCodeDuplicateAsync("S1")).ReturnsAsync(true);
            await Assert.ThrowsAsync<DuplicateException>(() => _service.AddAsync(dto));
        }

        [Fact]
        public async Task AddAsync_WhenPhoneDuplicate_Throws()
        {
            var dto = new SupplierDto { Code = "S1", Phone1 = "0912" };
            _repositoryMock.Setup(r => r.IsCodeDuplicateAsync("S1")).ReturnsAsync(false);
            _repositoryMock.Setup(r => r.IsPhoneDuplicateAsync("0912")).ReturnsAsync(true);
            await Assert.ThrowsAsync<DuplicateException>(() => _service.AddAsync(dto));
        }

        [Fact]
        public async Task DeleteAsync_WhenHasPurchaseInvoices_Throws()
        {
            _repositoryMock.Setup(r => r.HasPurchaseInvoicesAsync(1)).ReturnsAsync(true);
            await Assert.ThrowsAsync<BusinessRuleException>(() => _service.DeleteAsync(1));
        }
    }
}