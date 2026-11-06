using ConstructionManager.Api.Application.DTOs;
using ConstructionManager.Api.Application.Interfaces.Services;
using ConstructionManager.Api.Domain.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ConstructionManager.Api.Presentation.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class ProjectController : ControllerBase
    {
        private readonly IProjectService _service;

        public ProjectController(IProjectService service)
        {
            _service = service;
        }

        #region CRUD
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var projects = await _service.GetAllAsync();
            return Ok(projects);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] ProjectDto dto)
        {
            await _service.AddAsync(dto);
            return Ok();
        }

        [HttpPut]
        public async Task<IActionResult> Update([FromBody] ProjectDto dto)
        {
            await _service.UpdateAsync(dto);
            return Ok();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            await _service.DeleteAsync(id);
            return Ok();
        }
        #endregion

        #region Search
        [HttpGet("search-by-code/{code}")]
        public async Task<IActionResult> SearchByCode(string code)
        {
            var project = await _service.SearchByCodeAsync(code);
            return Ok(project);
        }

        [HttpGet("search-by-name/{name}")]
        public async Task<IActionResult> SearchByName(string name)
        {
            var projects = await _service.SearchByNameAsync(name);
            return Ok(projects);
        }
        #endregion

        #region Filter
        [HttpGet("by-status/{status}")]
        public async Task<IActionResult> GetByStatus(ProjectStatus status)
        {
            var projects = await _service.GetByStatusAsync(status);
            return Ok(projects);
        }

        [HttpGet("by-customer/{customerId}")]
        public async Task<IActionResult> GetByCustomer(int customerId)
        {
            var projects = await _service.GetByCustomerAsync(customerId);
            return Ok(projects);
        }

        [HttpGet("by-date-range")]
        public async Task<IActionResult> GetByDateRange([FromQuery] DateTime from, [FromQuery] DateTime to)
        {
            var projects = await _service.GetByDateRangeAsync(from, to);
            return Ok(projects);
        }
        #endregion
    }
}