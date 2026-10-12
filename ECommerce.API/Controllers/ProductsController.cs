using ECommerce.Application.DTOs.Files;
using ECommerce.Application.DTOs.Products;
using ECommerce.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ECommerce.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    //[Authorize(Roles = "Admin")]
    public class ProductsController : ControllerBase
    {
        private readonly IProductService _service;
        private readonly IImageService _imageService;
        private readonly IUnitOfWork unitOfWork;

        public ProductsController(IProductService productService, IImageService imageService, IUnitOfWork unitOfWork)
        {
            _service = productService;
            _imageService = imageService;
            this.unitOfWork = unitOfWork;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var products = await _service.GetAllAsync();
            return products is null ? NotFound() : Ok(products);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id)
        {
            var product = await _service.GetAsync(id);
            return product is null ? NotFound() : Ok(product);
        }

        [HttpPost]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> Create([FromForm] CreateProductRequest request, IFormFile mainImg)
        {
            await using var stream = mainImg.OpenReadStream();
            var upload = new FileUpload(stream, mainImg.FileName, mainImg.ContentType, mainImg.Length);

            var product = await _service.CreateAsync(request, upload);
            return CreatedAtAction(nameof(Get), new { id = product.Id }, product);
        }
        [HttpPatch("{id}")]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> Update(int id, [FromForm] UpdateProductRequest request, IFormFile? mainImg)
        {
            FileUpload? upload = null;
            Stream? stream = null;

            if (mainImg is not null)
            {
                stream = mainImg.OpenReadStream();
                upload = new FileUpload(stream, mainImg.FileName, mainImg.ContentType, mainImg.Length);
            }

            try
            {
                return await _service.UpdateAsync(id, request, upload) ? NoContent() : NotFound();
            }
            finally
            {
                if (stream is not null) await stream.DisposeAsync();
            }
        }
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
            => await _service.DeleteAsync(id) ? NoContent() : NotFound();


    }
}
