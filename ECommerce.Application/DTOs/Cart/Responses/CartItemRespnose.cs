using System;
using System.Collections.Generic;
using System.Text;

namespace ECommerce.Application.DTOs.Cart.Responses
{
    public record CartItemResponse
    {
        public int ProductId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string MainImg { get; set; } = string.Empty;
        public decimal UnitPrice { get; set; }     
        public int Count { get; set; }
        public decimal LineTotal { get; set; }
        public bool InStock { get; set; }
    }
}
