using ECommerce.Application.Constants;
using ECommerce.Application.DTOs.Promotions.Requests;
using ECommerce.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ECommerce.API.Controllers
{
    [Route("api/admin/promotions")]
    [ApiController]
    [Authorize(Roles = Roles.Admin)]
    public class AdminPromotionsController : ControllerBase
    {
        private readonly IAdminPromotionService _service;
        public AdminPromotionsController(IAdminPromotionService service) => _service = service;

        [HttpGet]
        public async Task<IActionResult> GetAll() => Ok(await _service.GetAllAsync());

        [HttpPost]
        public async Task<IActionResult> Create(CreatePromotionRequest request)
        {
            var (promo, error) = await _service.CreateAsync(request);
            return promo is null ? BadRequest(new { error }) : Created($"api/admin/promotions/{promo.Id}", promo);
        }

        [HttpPatch("{id}/active")]
        public async Task<IActionResult> SetActive(int id, [FromQuery] bool isActive)
            => await _service.SetActiveAsync(id, isActive) ? NoContent() : NotFound();
    }
}
