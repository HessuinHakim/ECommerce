using ECommerce.Application.DTOs.Identities.Requests;
using ECommerce.Application.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;

namespace ECommerce.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;
        public AuthController(IAuthService authService) => _authService = authService;

        [HttpPost("register")]
        public async Task<IActionResult> Register(RegisterRequest request)
        {
            var result = await _authService.RegisterAsync(request);
            return result.Succeeded ? Ok(result) : BadRequest(result);
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginRequest request)
        {
            var result = await _authService.LoginAsync(request);
            return result.Succeeded ? Ok(result) : Unauthorized(result);
        }
        [HttpGet("confirm-email")]
        public async Task<IActionResult> ConfirmEmail([FromQuery] ConfirmEmailRequest request)
        => await _authService.ConfirmEmailAsync(request)
            ? Ok("Email confirmed")
            : BadRequest("Invalid or expired link");

        [EnableRateLimiting("email")]
        [HttpPost("resend-confirmation")]
        public async Task<IActionResult> ResendConfirmationAsync(ResendEmailConfirmationRequest request)
        {
            await _authService.ResendEmailConfirmation(request);
            return Ok(new { message = "If the email exists and isn't confirmed, a new confirmation link has been sent." });
        }




        [EnableRateLimiting("email")]
        [HttpPost("forgot-password")]
        public async Task<IActionResult> ForgotPassword(ForgotPasswordRequest request)
        {
            await _authService.ForgotPasswordAsync(request);
            return Ok(new { message = "If the email exists, a reset code has been sent." });
        }

        [EnableRateLimiting("email")]
        [HttpPost("reset-password")]
        public async Task<IActionResult> ResetPassword(ResetPasswordRequest request)
        {
            var result = await _authService.ResetPasswordAsync(request);
            return result.Succeeded ? Ok(new { message = "Password has been reset." }) : BadRequest(result);
        }
    }
}
