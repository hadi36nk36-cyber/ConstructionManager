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
    public class ProjectServiceTests
    {
        private readonly Mock<IProjectRepository> _repositoryMock;
        private readonly Mock<IMapper> _mapperMock;
        private readonly Mock<ILogger<ProjectService>> _loggerMock;
        private readonly ProjectService _service;

        public ProjectServiceTests()
        {
            _repositoryMock = new Mock<IProjectRepository>();
            _mapperMock = new Mock<IMapper>();
            _loggerMock = new Mock<ILogger<ProjectService>>();
            _service = new ProjectService(_repositoryMock.Object, _mapperMock.Object, _loggerMock.Object);
        }

        [Fact]
        public async Task AddAsync_WhenEndDateBeforeStartDate_Throws()
        {
            var dto = new ProjectDto { StartDate = DateTime.Now, EndDate = DateTime.Now.AddDays(-1) };
            await Assert.ThrowsAsync<ValidationException>(() => _service.AddAsync(dto));
        }

        [Fact]
        public async Task DeleteAsync_WhenHasAssignments_Throws()
        {
            _repositoryMock.Setup(r => r.HasProjectAssignmentsAsync(1)).ReturnsAsync(true);
            await Assert.ThrowsAsync<BusinessRuleException>(() => _service.DeleteAsync(1));
        }

        [Fact]
        public async Task DeleteAsync_WhenHasInvoices_Throws()
        {
            _repositoryMock.Setup(r => r.HasProjectAssignmentsAsync(1)).ReturnsAsync(false);
            _repositoryMock.Setup(r => r.HasInvoicesAsync(1)).ReturnsAsync(true);
            await Assert.ThrowsAsync<BusinessRuleException>(() => _service.DeleteAsync(1));
        }

        [Fact]
        public async Task DeleteAsync_WhenHasPurchaseInvoices_Throws()
        {
            _repositoryMock.Setup(r => r.HasProjectAssignmentsAsync(1)).ReturnsAsync(false);
            _repositoryMock.Setup(r => r.HasInvoicesAsync(1)).ReturnsAsync(false);
            _repositoryMock.Setup(r => r.HasPurchaseInvoicesAsync(1)).ReturnsAsync(true);
            await Assert.ThrowsAsync<BusinessRuleException>(() => _service.DeleteAsync(1));
        }
    }
}