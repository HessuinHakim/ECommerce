using System;
using System.Collections.Generic;
using System.Text;

namespace ECommerce.Application.DTOs.Identities.Responses
{
    public record AuthResponse
    {
        public bool Succeeded { get; set; }
        public string? Token { get; set; }
        public DateTime? ExpiresAt { get; set; }
        public string? Email { get; set; }
        public IList<string> Roles { get; set; } = new List<string>();
        public IEnumerable<string> Errors { get; set; } = Enumerable.Empty<string>();
    }
}
