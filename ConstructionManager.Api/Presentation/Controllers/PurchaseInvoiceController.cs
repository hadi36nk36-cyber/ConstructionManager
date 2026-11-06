using ConstructionManager.Api.Application.DTOs;
using ConstructionManager.Api.Application.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ConstructionManager.Api.Presentation.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class PurchaseInvoiceController : ControllerBase
    {
        private readonly IPurchaseInvoiceService _service;

        public PurchaseInvoiceController(IPurchaseInvoiceService service)
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
        public async Task<IActionResult> Create([FromBody] PurchaseInvoiceDto dto)
        {
            await _service.AddAsync(dto);
            return Ok();
        }

        [HttpPut]
        public async Task<IActionResult> Update([FromBody] PurchaseInvoiceDto dto)
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
        [HttpGet("by-date")]
        public async Task<IActionResult> GetByDate([FromQuery] DateTime date)
        {
            var invoices = await _service.GetByInvoiceDateAsync(date);
            return Ok(invoices);
        }
        #endregion
    }
}