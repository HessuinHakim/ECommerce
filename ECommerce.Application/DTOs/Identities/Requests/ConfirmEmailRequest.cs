using System;
using System.Collections.Generic;
using System.Text;

namespace ECommerce.Application.DTOs.Identities.Requests
{
    public record ConfirmEmailRequest
    {
        public string UserId { get; init; } = string.Empty;
        public string Token { get; init; } = string.Empty;

    }
}
