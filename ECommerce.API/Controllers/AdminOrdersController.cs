using ECommerce.Application.Constants;
using ECommerce.Application.DTOs.Orders.Requests;
using ECommerce.Application.Interfaces;
using ECommerce.Domain.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ECommerce.API.Controllers
{
    [Route("api/admin/orders")]
    [ApiController]
    [Authorize(Roles = Roles.Admin)]
    public class AdminOrdersController : ControllerBase
    {
        private readonly IAdminOrderService _service;
        public AdminOrdersController(IAdminOrderService service) => _service = service;

        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] OrderStatus? status, int page = 1, int pageSize = 20)
            => Ok(await _service.GetAllAsync(status, page, Math.Clamp(pageSize, 1, 50)));

        [HttpPatch("{id}/status")]
        public async Task<IActionResult> UpdateStatus(int id, UpdateOrderStatusRequest request)
        {
            var (ok, error) = await _service.UpdateStatusAsync(id, request.Status);
            return ok ? NoContent() : BadRequest(new { error });
        }
    }
}
