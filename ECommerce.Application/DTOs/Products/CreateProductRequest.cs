using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace ECommerce.Application.DTOs.Products
{
    public record CreateProductRequest
    {
        [Required, MaxLength(200)]
        public string Name { get; set; } = string.Empty;

        public string? Description { get; set; }

        [Range(0.01, 1_000_000)]
        public decimal Price { get; set; }

        [Range(0, 100)]
        public decimal Discount { get; set; }

        [Range(0, int.MaxValue)]
        public int Quantity { get; set; }

        [Required] public int CategoryId { get; set; }
        [Required] public int BrandId { get; set; }
    }
}
