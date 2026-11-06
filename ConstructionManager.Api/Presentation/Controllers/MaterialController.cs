using ConstructionManager.Api.Application.DTOs;
using ConstructionManager.Api.Application.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ConstructionManager.Api.Presentation.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class MaterialController : ControllerBase
    {
        private readonly IMaterialService _service;

        public MaterialController(IMaterialService service)
        {
            _service = service;
        }

        #region CRUD
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var materials = await _service.GetAllAsync();
            return Ok(materials);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] MaterialDto dto)
        {
            await _service.AddAsync(dto);
            return Ok();
        }

        [HttpPut]
        public async Task<IActionResult> Update([FromBody] MaterialDto dto)
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
            var material = await _service.SearchByCodeAsync(code);
            return Ok(material);
        }

        [HttpGet("search-by-name/{name}")]
        public async Task<IActionResult> SearchByName(string name)
        {
            var materials = await _service.SearchByNameAsync(name);
            return Ok(materials);
        }
        #endregion

        #region Filter
        [HttpGet("by-category/{category}")]
        public async Task<IActionResult> GetByCategory(string category)
        {
            var materials = await _service.GetByCategoryAsync(category);
            return Ok(materials);
        }
        #endregion
    }
}