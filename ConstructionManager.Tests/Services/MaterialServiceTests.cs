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
    public class MaterialServiceTests
    {
        private readonly Mock<IMaterialRepository> _repositoryMock;
        private readonly Mock<IMapper> _mapperMock;
        private readonly Mock<ILogger<MaterialService>> _loggerMock;
        private readonly MaterialService _service;

        public MaterialServiceTests()
        {
            _repositoryMock = new Mock<IMaterialRepository>();
            _mapperMock = new Mock<IMapper>();
            _loggerMock = new Mock<ILogger<MaterialService>>();
            _service = new MaterialService(_repositoryMock.Object, _mapperMock.Object, _loggerMock.Object);
        }

        [Fact]
        public async Task AddAsync_WhenCodeDuplicate_Throws()
        {
            var dto = new MaterialDto { Code = "M1" };
            _repositoryMock.Setup(r => r.IsCodeDuplicateAsync("M1")).ReturnsAsync(true);
            await Assert.ThrowsAsync<DuplicateException>(() => _service.AddAsync(dto));
        }

        [Fact]
        public async Task AddAsync_WhenQuantityNegative_Throws()
        {
            var dto = new MaterialDto { Quantity = -1 };
            await Assert.ThrowsAsync<ValidationException>(() => _service.AddAsync(dto));
        }
    }
}