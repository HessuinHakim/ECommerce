using System;
using System.Collections.Generic;
using System.Text;

namespace ECommerce.Application.DTOs.Orders.Responses
{
    public record OrderResponse
    {
        public int Id { get; set; }
        public string Status { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public decimal Subtotal { get; set; }
        public decimal DiscountAmount { get; set; }
        public decimal Total { get; set; }
        public List<OrderItemResponse> Items { get; set; } = new();
    }
}
