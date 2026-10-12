using System.Security.Claims;
using ECommerce.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ECommerce.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PaymentsController : ControllerBase
    {
        private readonly IPaymentService _payments;
        private readonly IOrderService _orders;

        public PaymentsController(IPaymentService payments, IOrderService orders)
        {
            _payments = payments;
            _orders = orders;
        }

        private string UserId => User.FindFirstValue(ClaimTypes.NameIdentifier)!;

        [Authorize]
        [HttpPost("{orderId}/pay")]
        public async Task<IActionResult> Pay(int orderId)
        {
            var order = await _orders.GetMyOrderAsync(UserId, orderId);
            if (order is null) return NotFound();
            if (order.Status != "Pending") return BadRequest(new { error = "Order is not payable." });

            var url = await _payments.CreateCheckoutSessionAsync(orderId);
            return Ok(new { url });
        }

        [AllowAnonymous]
        [HttpPost("webhook")]
        public async Task<IActionResult> Webhook()
        {
            var payload = await new StreamReader(Request.Body).ReadToEndAsync();
            var signature = Request.Headers["Stripe-Signature"].ToString();

            return await _payments.HandleWebhookAsync(payload, signature) ? Ok() : BadRequest();
        }

        [HttpGet("success")] public IActionResult Success() => Ok("Payment received. Your order will be confirmed shortly.");
        [HttpGet("cancel")] public IActionResult Cancel() => Ok("Payment cancelled.");
    }
}
