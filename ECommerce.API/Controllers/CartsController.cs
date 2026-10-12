using System.Security.Claims;
using ECommerce.Application.DTOs.Cart.Requests;
using ECommerce.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ECommerce.API.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class CartsController : ControllerBase
    {
        private readonly ICartService _service;
        public CartsController(ICartService service) => _service = service;

        private string UserId => User.FindFirstValue(ClaimTypes.NameIdentifier)!;

        [HttpGet]
        public async Task<IActionResult> Get() => Ok(await _service.GetAsync(UserId));

        [HttpPost]
        public async Task<IActionResult> Add(AddToCartRequest request)
        {
            var (ok, error) = await _service.AddAsync(UserId, request);
            return ok ? NoContent() : BadRequest(new { error });
        }

        [HttpPut("{productId}")]
        public async Task<IActionResult> Update(int productId, UpdateCartItemRequest request)
        {
            var (ok, error) = await _service.UpdateCountAsync(UserId, productId, request.Count);
            return ok ? NoContent() : BadRequest(new { error });
        }

        [HttpDelete("{productId}")]
        public async Task<IActionResult> Remove(int productId)
            => await _service.RemoveAsync(UserId, productId) ? NoContent() : NotFound();

        [HttpDelete]
        public async Task<IActionResult> Clear()
        {
            await _service.ClearAsync(UserId);
            return NoContent();
        }
    }
}
