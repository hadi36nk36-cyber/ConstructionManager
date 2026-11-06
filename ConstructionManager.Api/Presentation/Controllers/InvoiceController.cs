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
    public class InvoiceController : ControllerBase
    {
        private readonly IInvoiceService _service;

        public InvoiceController(IInvoiceService service)
        {
            _service = service;
        }

        #region CRUD
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var invoices = await _service.GetAllAsync();
            return Ok(invoices);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] InvoiceDto dto)
        {
            await _service.AddAsync(dto);
            return Ok();
        }

        [HttpPut]
        public async Task<IActionResult> Update([FromBody] InvoiceDto dto)
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
            var invoice = await _service.SearchByCodeAsync(code);
            return Ok(invoice);
        }

        [HttpGet("search-by-tracking-code/{trackingCode}")]
        public async Task<IActionResult> SearchByTrackingCode(string trackingCode)
        {
            var invoice = await _service.SearchByTrackingCodeAsync(trackingCode);
            return Ok(invoice);
        }
        #endregion

        #region Filter
        [HttpGet("by-status/{status}")]
        public async Task<IActionResult> GetByStatus(InvoiceStatus status)
        {
            var invoices = await _service.GetByStatusAsync(status);
            return Ok(invoices);
        }

        [HttpGet("by-date-range")]
        public async Task<IActionResult> GetByDateRange([FromQuery] DateTime from, [FromQuery] DateTime to)
        {
            var invoices = await _service.GetByDateRangeAsync(from, to);
            return Ok(invoices);
        }
        #endregion
    }
}