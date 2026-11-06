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
    public class PaymentServiceTests
    {
        private readonly Mock<IPaymentRepository> _repositoryMock;
        private readonly Mock<IMapper> _mapperMock;
        private readonly Mock<ILogger<PaymentService>> _loggerMock;
        private readonly PaymentService _service;

        public PaymentServiceTests()
        {
            _repositoryMock = new Mock<IPaymentRepository>();
            _mapperMock = new Mock<IMapper>();
            _loggerMock = new Mock<ILogger<PaymentService>>();
            _service = new PaymentService(_repositoryMock.Object, _mapperMock.Object, _loggerMock.Object);
        }

        [Fact]
        public async Task AddAsync_WhenNoReferences_Throws()
        {
            var dto = new PaymentDto { EmployeeId = null, PurchaseInvoiceId = null, EquipmentId = null , Amount = 100 };
            await Assert.ThrowsAsync<BusinessRuleException>(() => _service.AddAsync(dto));
        }

        [Fact]
        public async Task AddAsync_WhenAmountIsNegative_Throws()
        {
            var dto = new PaymentDto { EmployeeId = 1, Amount = -10 };
            await Assert.ThrowsAsync<ValidationException>(() => _service.AddAsync(dto));
        }
    }
}