using System;
using System.Collections.Generic;
using System.Text;

namespace ECommerce.Application.DTOs.Brands
{
    public record BrandDTO
    {
        public int Id { get; set; }

        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; } = string.Empty;
        public bool Status { get; set; } = true;

    }
}
