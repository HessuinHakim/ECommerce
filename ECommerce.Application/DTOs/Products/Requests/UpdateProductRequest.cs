using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace ECommerce.Application.DTOs.Products.Requests
{
    public record UpdateProductRequest
    {
        [MaxLength(200)] public string? Name { get; set; }
        public string? Description { get; set; }
        [Range(0.01, 1_000_000)] public decimal? Price { get; set; }
        [Range(0, 100)] public decimal? Discount { get; set; }
        [Range(0, int.MaxValue)] public int? Quantity { get; set; }
        public bool? Status { get; set; }
        [Range(1, int.MaxValue)] public int? CategoryId { get; set; }
        [Range(1, int.MaxValue)] public int? BrandId { get; set; }

    }
}
