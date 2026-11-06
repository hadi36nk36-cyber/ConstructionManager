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
    public class EmployeeController : ControllerBase
    {
        private readonly IEmployeeService _service;

        public EmployeeController(IEmployeeService service)
        {
            _service = service;
        }

        #region CRUD
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var employees = await _service.GetAllAsync();
            return Ok(employees);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] EmployeeDto dto)
        {
            await _service.AddAsync(dto);
            return Ok();
        }

        [HttpPut]
        public async Task<IActionResult> Update([FromBody] EmployeeDto dto)
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
        [HttpGet("search-by-name/{name}")]
        public async Task<IActionResult> SearchByName(string name)
        {
            var employees = await _service.SearchByNameAsync(name);
            return Ok(employees);
        }

        [HttpGet("search-by-national-code/{nationalCode}")]
        public async Task<IActionResult> SearchByNationalCode(string nationalCode)
        {
            var employee = await _service.SearchByNationalCodeAsync(nationalCode);
            return Ok(employee);
        }

        [HttpGet("search-by-employee-code/{employeeCode}")]
        public async Task<IActionResult> SearchByEmployeeCode(string employeeCode)
        {
            var employee = await _service.SearchByEmployeeCodeAsync(employeeCode);
            return Ok(employee);
        }
        #endregion

        #region Filter
        [HttpGet("by-status/{status}")]
        public async Task<IActionResult> GetByStatus(EmployeeStatus status)
        {
            var employees = await _service.GetByStatusAsync(status);
            return Ok(employees);
        }

        [HttpGet("by-speciality/{speciality}")]
        public async Task<IActionResult> GetBySpeciality(string speciality)
        {
            var employees = await _service.GetBySpecialityAsync(speciality);
            return Ok(employees);
        }
        #endregion
    }
}