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
    public class PaymentController : ControllerBase
    {
        private readonly IPaymentService _service;

        public PaymentController(IPaymentService service)
        {
            _service = service;
        }

        #region CRUD
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var payments = await _service.GetAllAsync();
            return Ok(payments);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] PaymentDto dto)
        {
            await _service.AddAsync(dto);
            return Ok();
        }

        [HttpPut]
        public async Task<IActionResult> Update([FromBody] PaymentDto dto)
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
            var payment = await _service.SearchByCodeAsync(code);
            return Ok(payment);
        }

        [HttpGet("search-by-tracking-code/{trackingCode}")]
        public async Task<IActionResult> SearchByTrackingCode(string trackingCode)
        {
            var payment = await _service.SearchByTrackingCodeAsync(trackingCode);
            return Ok(payment);
        }
        #endregion

        #region Filter
        [HttpGet("by-type/{type}")]
        public async Task<IActionResult> GetByType(PaymentType type)
        {
            var payments = await _service.GetByTypeAsync(type);
            return Ok(payments);
        }

        [HttpGet("by-date")]
        public async Task<IActionResult> GetByDate([FromQuery] DateTime date)
        {
            var payments = await _service.GetByDateAsync(date);
            return Ok(payments);
        }
        #endregion
    }
}