using ConstructionManager.Api.Application.DTOs;
using ConstructionManager.Api.Application.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ConstructionManager.Api.Presentation.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class ProjectAssignmentController : ControllerBase
    {
        private readonly IProjectAssignmentService _service;

        public ProjectAssignmentController(IProjectAssignmentService service)
        {
            _service = service;
        }

        #region CRUD
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var assignments = await _service.GetAllAsync();
            return Ok(assignments);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] ProjectAssignmentDto dto)
        {
            await _service.AddAsync(dto);
            return Ok();
        }

        [HttpPut]
        public async Task<IActionResult> Update([FromBody] ProjectAssignmentDto dto)
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
        [HttpGet("by-employee/{employeeId}")]
        public async Task<IActionResult> SearchByEmployee(int employeeId)
        {
            var assignments = await _service.SearchByEmployeeAsync(employeeId);
            return Ok(assignments);
        }

        [HttpGet("by-project/{projectId}")]
        public async Task<IActionResult> SearchByProject(int projectId)
        {
            var assignments = await _service.SearchByProjectAsync(projectId);
            return Ok(assignments);
        }
        #endregion

        #region Filter
        [HttpGet("by-role/{role}")]
        public async Task<IActionResult> GetByRole(string role)
        {
            var assignments = await _service.GetByRoleAsync(role);
            return Ok(assignments);
        }

        [HttpGet("by-date")]
        public async Task<IActionResult> GetByDate([FromQuery] DateTime date)
        {
            var assignments = await _service.GetByDateAsync(date);
            return Ok(assignments);
        }
        #endregion
    }
}