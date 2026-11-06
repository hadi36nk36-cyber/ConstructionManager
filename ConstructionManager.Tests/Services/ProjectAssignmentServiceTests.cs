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
    public class ProjectAssignmentServiceTests
    {
        private readonly Mock<IProjectAssignmentRepository> _repositoryMock;
        private readonly Mock<IMapper> _mapperMock;
        private readonly Mock<ILogger<ProjectAssignmentService>> _loggerMock;
        private readonly ProjectAssignmentService _service;

        public ProjectAssignmentServiceTests()
        {
            _repositoryMock = new Mock<IProjectAssignmentRepository>();
            _mapperMock = new Mock<IMapper>();
            _loggerMock = new Mock<ILogger<ProjectAssignmentService>>();
            _service = new ProjectAssignmentService(_repositoryMock.Object, _mapperMock.Object, _loggerMock.Object);
        }

        [Fact]
        public async Task AddAsync_WhenDuplicate_Throws()
        {
            var dto = new ProjectAssignmentDto { ProjectId = 1, EmployeeId = 1 };
            _repositoryMock.Setup(r => r.IsDuplicateAsync(1, 1)).ReturnsAsync(true);
            await Assert.ThrowsAsync<DuplicateException>(() => _service.AddAsync(dto));
        }

        [Fact]
        public async Task AddAsync_WhenEndDateBeforeJoinDate_Throws()
        {
            var dto = new ProjectAssignmentDto { JoinDate = DateTime.Now, EndDate = DateTime.Now.AddDays(-1) };
            _repositoryMock.Setup(r => r.IsDuplicateAsync(It.IsAny<int>(), It.IsAny<int>())).ReturnsAsync(false);
            await Assert.ThrowsAsync<ValidationException>(() => _service.AddAsync(dto));
        }
    }
}