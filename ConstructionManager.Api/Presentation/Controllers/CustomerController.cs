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
    public class CustomerController : ControllerBase
    {
        private readonly ICustomerService _service;

        public CustomerController(ICustomerService service)
        {
            _service = service;
        }

        #region CRUD
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var customers = await _service.GetAllAsync();
            return Ok(customers);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CustomerDto dto)
        {
            await _service.AddAsync(dto);
            return Ok();
        }

        [HttpPut]
        public async Task<IActionResult> Update([FromBody] CustomerDto dto)
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
            var customer = await _service.SearchByCodeAsync(code);
            return Ok(customer);
        }

        [HttpGet("search-by-name/{name}")]
        public async Task<IActionResult> SearchByName(string name)
        {
            var customers = await _service.SearchByNameAsync(name);
            return Ok(customers);
        }

        [HttpGet("search-by-phone/{phone}")]
        public async Task<IActionResult> SearchByPhone(string phone)
        {
            var customers = await _service.SearchByPhoneAsync(phone);
            return Ok(customers);
        }
        #endregion

        #region Filter
        [HttpGet("by-type/{type}")]
        public async Task<IActionResult> GetByType(CustomerType type)
        {
            var customers = await _service.GetByTypeAsync(type);
            return Ok(customers);
        }
        #endregion
    }
}