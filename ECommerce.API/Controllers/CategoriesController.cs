using ECommerce.Application.DTOs.Categories;
using ECommerce.Application.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ECommerce.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CategoriesController : ControllerBase
    {
        private readonly ICategoryService _service;

        public CategoriesController(ICategoryService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var categories =await _service.GetAllAsync();   
            return Ok(categories);
        }
        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id)
        {
            var category = await _service.GetByIdAsync(id);
            return category is null ? NotFound() : Ok(category);
        }

        [HttpPost]
        public async Task<IActionResult> Create(CreateCategoryRequest createCategoryRequest)
        { 
            var category  = await _service.CreateAsync(createCategoryRequest);

            return CreatedAtAction(nameof(Get), new { id = category.Id }, category);

        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, UpdateCategoryRequest updateCategoryRequest)
            => await _service.UpdateAsync(id, updateCategoryRequest) ? NoContent() : NotFound();

        [HttpPatch("{id}")]
        public async Task<IActionResult> Patch(int id, PatchCategoryRequest patchCategoryRequest)
            => await _service.PatchAsync(id, patchCategoryRequest) ? NoContent() : NotFound();

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
            => await _service.DeleteAsync(id) ? NoContent() : NotFound();
    }

}
