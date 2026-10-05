using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace ECommerce.Application.DTOs.Identities.Requests
{
    public record ForgotPasswordRequest
    {
        [Required,EmailAddress]
        public string Email { get; init; } = string.Empty;
    }
}
