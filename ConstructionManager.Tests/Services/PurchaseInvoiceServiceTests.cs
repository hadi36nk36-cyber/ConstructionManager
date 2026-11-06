using AutoMapper;
using ConstructionManager.Api.Application.DTOs;
using ConstructionManager.Api.Application.Interfaces.Repositories;
using ConstructionManager.Api.Application.Services;
using ConstructionManager.Api.Domain.Entities;
using ConstructionManager.Api.Domain.Enums;
using ConstructionManager.Api.Domain.Exceptions;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace ConstructionManager.Tests.Services
{
    public class PurchaseInvoiceServiceTests
    {
        private readonly Mock<IPurchaseInvoiceRepository> _repositoryMock;
        private readonly Mock<IMapper> _mapperMock;
        private readonly Mock<ILogger<PurchaseInvoiceService>> _loggerMock;
        private readonly PurchaseInvoiceService _service;

        public PurchaseInvoiceServiceTests()
        {
            _repositoryMock = new Mock<IPurchaseInvoiceRepository>();
            _mapperMock = new Mock<IMapper>();
            _loggerMock = new Mock<ILogger<PurchaseInvoiceService>>();
            _service = new PurchaseInvoiceService(_repositoryMock.Object, _mapperMock.Object, _loggerMock.Object);
        }

        [Fact]
        public async Task AddAsync_WhenTrackingCodeDuplicate_Throws()
        {
            var dto = new PurchaseInvoiceDto { Code = "P1", TrackingCode = "T1" };
            _repositoryMock.Setup(r => r.IsCodeDuplicateAsync("P1")).ReturnsAsync(false);
            _repositoryMock.Setup(r => r.IsTrackingCodeDuplicateAsync("T1")).ReturnsAsync(true);
            await Assert.ThrowsAsync<DuplicateException>(() => _service.AddAsync(dto));
        }

        [Fact]
        public async Task UpdateAsync_WhenCancelled_Throws()
        {
            var dto = new PurchaseInvoiceDto { Status = PurchaseInvoiceStatus.Cancelled };
            await Assert.ThrowsAsync<OperationNotAllowedException>(() => _service.UpdateAsync(dto));
        }

        [Fact]
        public async Task DeleteAsync_WhenHasDetails_Throws()
        {
            _repositoryMock.Setup(r => r.HasDetailsAsync(1)).ReturnsAsync(true);
            await Assert.ThrowsAsync<BusinessRuleException>(() => _service.DeleteAsync(1));
        }

        [Fact]
        public async Task DeleteAsync_WhenHasPayments_Throws()
        {
            _repositoryMock.Setup(r => r.HasDetailsAsync(1)).ReturnsAsync(false);
            _repositoryMock.Setup(r => r.HasPaymentsAsync(1)).ReturnsAsync(true);
            await Assert.ThrowsAsync<BusinessRuleException>(() => _service.DeleteAsync(1));
        }
    }
}