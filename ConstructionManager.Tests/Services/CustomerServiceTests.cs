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
    public class CustomerServiceTests
    {
        private readonly Mock<ICustomerRepository> _repositoryMock;
        private readonly Mock<IMapper> _mapperMock;
        private readonly Mock<ILogger<CustomerService>> _loggerMock;
        private readonly CustomerService _service;

        public CustomerServiceTests()
        {
            _repositoryMock = new Mock<ICustomerRepository>();
            _mapperMock = new Mock<IMapper>();
            _loggerMock = new Mock<ILogger<CustomerService>>();
            _service = new CustomerService(_repositoryMock.Object, _mapperMock.Object, _loggerMock.Object);
        }

        [Fact]
        public async Task GetAllAsync_WhenCalled_ReturnsListOfCustomers()
        {
            var customers = new List<Customer> { new Customer { Id = 1, Name = "Test" } };
            var dtos = new List<CustomerDto> { new CustomerDto { Id = 1, Name = "Test" } };

            _repositoryMock.Setup(r => r.GetAllAsync()).ReturnsAsync(customers);
            _mapperMock.Setup(m => m.Map<List<CustomerDto>>(customers)).Returns(dtos);

            var result = await _service.GetAllAsync();

            Assert.NotNull(result);
            Assert.Single(result);
        }

        [Fact]
        public async Task AddAsync_WhenValid_AddsCustomer()
        {
            var dto = new CustomerDto { Code = "C-01", Name = "Ali", Phone1 = "0912" };
            var entity = new Customer { Code = "C-01", Name = "Ali" };

            _repositoryMock.Setup(r => r.IsCodeDuplicateAsync(It.IsAny<string>())).ReturnsAsync(false);
            _repositoryMock.Setup(r => r.IsPhoneDuplicateAsync(It.IsAny<string>())).ReturnsAsync(false);
            _mapperMock.Setup(m => m.Map<Customer>(dto)).Returns(entity);

            await _service.AddAsync(dto);

            _repositoryMock.Verify(r => r.AddAsync(entity), Times.Once);
        }

        [Fact]
        public async Task AddAsync_WhenCodeDuplicate_Throws()
        {
            var dto = new CustomerDto { Code = "C-01" };
            _repositoryMock.Setup(r => r.IsCodeDuplicateAsync("C-01")).ReturnsAsync(true);

            await Assert.ThrowsAsync<DuplicateException>(() => _service.AddAsync(dto));
        }

        [Fact]
        public async Task AddAsync_WhenPhoneDuplicate_Throws()
        {
            var dto = new CustomerDto { Code = "C-02", Phone1 = "0912" };
            // کد تکراری نیست
            _repositoryMock.Setup(r => r.IsCodeDuplicateAsync("C-02")).ReturnsAsync(false);
            // ولی شماره تکراریه
            _repositoryMock.Setup(r => r.IsPhoneDuplicateAsync("0912")).ReturnsAsync(true);

            await Assert.ThrowsAsync<DuplicateException>(() => _service.AddAsync(dto));
        }

        [Fact]
        public async Task DeleteAsync_WhenNoDependencies_DeletesCustomer()
        {
            _repositoryMock.Setup(r => r.HasProjectsAsync(1)).ReturnsAsync(false);
            _repositoryMock.Setup(r => r.HasInvoicesAsync(1)).ReturnsAsync(false);

            await _service.DeleteAsync(1);

            _repositoryMock.Verify(r => r.DeleteAsync(1), Times.Once);
        }

        [Fact]
        public async Task DeleteAsync_WhenHasProjects_Throws()
        {
            _repositoryMock.Setup(r => r.HasProjectsAsync(1)).ReturnsAsync(true);
            await Assert.ThrowsAsync<BusinessRuleException>(() => _service.DeleteAsync(1));
        }

        [Fact]
        public async Task DeleteAsync_WhenHasInvoices_Throws()
        {
            _repositoryMock.Setup(r => r.HasProjectsAsync(1)).ReturnsAsync(false);
            _repositoryMock.Setup(r => r.HasInvoicesAsync(1)).ReturnsAsync(true);

            await Assert.ThrowsAsync<BusinessRuleException>(() => _service.DeleteAsync(1));
        }
    }
}