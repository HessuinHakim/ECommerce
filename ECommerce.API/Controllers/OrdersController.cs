using System.Security.Claims;
using ECommerce.Application.DTOs.Orders.Requests;
using ECommerce.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ECommerce.API.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class OrdersController : ControllerBase
    {
        private readonly IOrderService _service;
        public OrdersController(IOrderService service) => _service = service;

        private string UserId => User.FindFirstValue(ClaimTypes.NameIdentifier)!;

        [HttpPost("checkout")]
        public async Task<IActionResult> Checkout([FromBody] CheckoutRequest? request)
        {
            var (order, error) = await _service.CheckoutAsync(UserId, request ?? new CheckoutRequest());
            return order is null
                ? BadRequest(new { error })
                : CreatedAtAction(nameof(Get), new { id = order.Id }, order);
        }

        [HttpGet]
        public async Task<IActionResult> GetMine() => Ok(await _service.GetMyOrdersAsync(UserId));

        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id)
        {
            var order = await _service.GetMyOrderAsync(UserId, id);
            return order is null ? NotFound() : Ok(order);
        }
    }
}
