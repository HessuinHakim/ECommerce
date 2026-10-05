using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace ECommerce.Application.DTOs.Identities.Requests
{
    public record ResetPasswordRequest
    {
        [Required, EmailAddress]
        public string Email { get; init; } = string.Empty;

        [Required, StringLength(6, MinimumLength = 6, ErrorMessage = "OTP must be 6 characters long.")]
        public string Otp { get; init; } = string.Empty;

        [Required, MinLength(8)]
        public string NewPassword { get; set; } = string.Empty;
    }
}
