using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace ECommerce.Application.DTOs.Brands
{
    public record UpdateBrandRequest
    {
        [Required(ErrorMessage = "Name is required")]
        [MaxLength(100, ErrorMessage = "Name should not exceed 100 characters")]
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; } = string.Empty;
        public bool Status { get; set; } = true;
    }
}
