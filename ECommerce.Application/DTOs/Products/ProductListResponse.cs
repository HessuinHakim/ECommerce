using System;
using System.Collections.Generic;
using System.Text;

namespace ECommerce.Application.DTOs.Products
{
    public record ProductListResponse
    {

        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string MainImg { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public decimal Discount { get; set; }
        public decimal FinalPrice { get; set; }
        public double Rate { get; set; }
        public bool Status { get; set; }
        public string CategoryName { get; set; } = string.Empty;
        public string BrandName { get; set; } = string.Empty;
    }
}
