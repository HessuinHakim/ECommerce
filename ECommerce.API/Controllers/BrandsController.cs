using ECommerce.Application.DTOs.Brands;
using ECommerce.Application.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ECommerce.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BrandsController : ControllerBase
    {
        private readonly IBrandService _service;

        public BrandsController(IBrandService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var brands = await _service.GetAllAsync();
            return Ok(brands);
        }
        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id)
        {
            var brands = await _service.GetByIdAsync(id);
            return brands is null ? NotFound() : Ok(brands);
        }

        [HttpPost]
        public async Task<IActionResult> Create(CreateBrandRequest createBrandRequest)
        {
            var brand = await _service.CreateAsync(createBrandRequest);

            return CreatedAtAction(nameof(Get), new { id = brand.Id }, brand);

        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, UpdateBrandRequest updateBrandRequest)
            => await _service.UpdateAsync(id, updateBrandRequest) ? NoContent() : NotFound();

        [HttpPatch("{id}")]
        public async Task<IActionResult> Patch(int id, PatchBrandRequest patchBrandRequest)
            => await _service.PatchAsync(id, patchBrandRequest) ? NoContent() : NotFound();

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
            => await _service.DeleteAsync(id) ? NoContent() : NotFound();
    }
}
