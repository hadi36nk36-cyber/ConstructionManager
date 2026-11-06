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
    public class EquipmentController : ControllerBase
    {
        private readonly IEquipmentService _service;

        public EquipmentController(IEquipmentService service)
        {
            _service = service;
        }

        #region CRUD
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var equipments = await _service.GetAllAsync();
            return Ok(equipments);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] EquipmentDto dto)
        {
            await _service.AddAsync(dto);
            return Ok();
        }

        [HttpPut]
        public async Task<IActionResult> Update([FromBody] EquipmentDto dto)
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
            var equipment = await _service.SearchByCodeAsync(code);
            return Ok(equipment);
        }

        [HttpGet("search-by-serial-number/{serialNumber}")]
        public async Task<IActionResult> SearchBySerialNumber(string serialNumber)
        {
            var equipment = await _service.SearchBySerialNumberAsync(serialNumber);
            return Ok(equipment);
        }
        #endregion

        #region Filter
        [HttpGet("by-type/{type}")]
        public async Task<IActionResult> GetByType(EquipmentType type)
        {
            var equipments = await _service.GetByTypeAsync(type);
            return Ok(equipments);
        }

        [HttpGet("by-status/{status}")]
        public async Task<IActionResult> GetByStatus(EquipmentStatus status)
        {
            var equipments = await _service.GetByStatusAsync(status);
            return Ok(equipments);
        }
        #endregion
    }
}