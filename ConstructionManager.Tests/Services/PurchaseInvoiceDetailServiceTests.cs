using AutoMapper;
using ConstructionManager.Api.Application.DTOs;
using ConstructionManager.Api.Application.Interfaces.Repositories;
using ConstructionManager.Api.Application.Services;
using ConstructionManager.Api.Domain.Exceptions;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace ConstructionManager.Tests.Services
{
    public class PurchaseInvoiceDetailServiceTests
    {
        private readonly Mock<IPurchaseInvoiceDetailRepository> _repositoryMock;
        private readonly Mock<IMapper> _mapperMock;
        private readonly Mock<ILogger<PurchaseInvoiceDetailService>> _loggerMock;
        private readonly PurchaseInvoiceDetailService _service;

        public PurchaseInvoiceDetailServiceTests()
        {
            _repositoryMock = new Mock<IPurchaseInvoiceDetailRepository>();
            _mapperMock = new Mock<IMapper>();
            _loggerMock = new Mock<ILogger<PurchaseInvoiceDetailService>>();
            _service = new PurchaseInvoiceDetailService(_repositoryMock.Object, _mapperMock.Object, _loggerMock.Object);
        }

        [Fact]
        public async Task AddAsync_WhenQuantityIsZero_Throws()
        {
            var dto = new PurchaseInvoiceDetailDto { Quantity = 0 };
            await Assert.ThrowsAsync<ValidationException>(() => _service.AddAsync(dto));
        }

        [Fact]
        public async Task AddAsync_WhenUnitPriceIsNegative_Throws()
        {
            var dto = new PurchaseInvoiceDetailDto { Quantity = 1, UnitPrice = -1 };
            await Assert.ThrowsAsync<ValidationException>(() => _service.AddAsync(dto));
        }
    }
}