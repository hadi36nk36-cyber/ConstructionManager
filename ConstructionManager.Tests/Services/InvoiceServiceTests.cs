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
    public class InvoiceServiceTests
    {
        private readonly Mock<IInvoiceRepository> _repositoryMock;
        private readonly Mock<IMapper> _mapperMock;
        private readonly Mock<ILogger<InvoiceService>> _loggerMock;
        private readonly InvoiceService _service;

        public InvoiceServiceTests()
        {
            _repositoryMock = new Mock<IInvoiceRepository>();
            _mapperMock = new Mock<IMapper>();
            _loggerMock = new Mock<ILogger<InvoiceService>>();
            _service = new InvoiceService(_repositoryMock.Object, _mapperMock.Object, _loggerMock.Object);
        }

        [Fact]
        public async Task AddAsync_WhenCodeDuplicate_Throws()
        {
            var dto = new InvoiceDto { Code = "I1" };
            _repositoryMock.Setup(r => r.IsCodeDuplicateAsync("I1")).ReturnsAsync(true);
            await Assert.ThrowsAsync<DuplicateException>(() => _service.AddAsync(dto));
        }

        [Fact]
        public async Task AddAsync_WhenTrackingCodeDuplicate_Throws()
        {
            var dto = new InvoiceDto { Code = "I1", TrackingCode = "T1" };
            _repositoryMock.Setup(r => r.IsCodeDuplicateAsync("I1")).ReturnsAsync(false);
            _repositoryMock.Setup(r => r.IsTrackingCodeDuplicateAsync("T1")).ReturnsAsync(true);
            await Assert.ThrowsAsync<DuplicateException>(() => _service.AddAsync(dto));
        }

        [Fact]
        public async Task AddAsync_WhenProjectHasInvoice_Throws()
        {
            var dto = new InvoiceDto { Code = "I1", TrackingCode = "T1", ProjectId = 5 };
            _repositoryMock.Setup(r => r.IsCodeDuplicateAsync(It.IsAny<string>())).ReturnsAsync(false);
            _repositoryMock.Setup(r => r.IsTrackingCodeDuplicateAsync(It.IsAny<string>())).ReturnsAsync(false);
            _repositoryMock.Setup(r => r.IsProjectIdDuplicateAsync(5)).ReturnsAsync(true);
            await Assert.ThrowsAsync<BusinessRuleException>(() => _service.AddAsync(dto));
        }

        [Fact]
        public async Task UpdateAsync_WhenCancelled_Throws()
        {
            var dto = new InvoiceDto { Status = InvoiceStatus.Cancelled };
            await Assert.ThrowsAsync<OperationNotAllowedException>(() => _service.UpdateAsync(dto));
        }

        [Fact]
        public async Task DeleteAsync_WhenHasPurchaseInvoices_Throws()
        {
            _repositoryMock.Setup(r => r.HasPurchaseInvoicesAsync(1)).ReturnsAsync(true);
            await Assert.ThrowsAsync<BusinessRuleException>(() => _service.DeleteAsync(1));
        }
    }
}