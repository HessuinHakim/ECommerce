using System;
using System.Collections.Generic;
using System.Text;

namespace ECommerce.Application.DTOs.Brands
{
    public record CreateBrandRequest
    {
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; } = string.Empty;
    }
}
