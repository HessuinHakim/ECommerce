using System;
using System.Collections.Generic;
using System.Text;

namespace ECommerce.Application.DTOs.Cart.Responses
{
    public record CartResponse
    {
        public List<CartItemResponse> Items { get; set; } = new();
        public decimal Total { get; set; }
    }
}
