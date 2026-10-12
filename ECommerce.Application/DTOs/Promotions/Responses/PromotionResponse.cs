using System;
using System.Collections.Generic;
using System.Text;

namespace ECommerce.Application.DTOs.Promotions.Responses
{
    public record PromotionResponse
    {
        public int Id { get; set; }
        public string Code { get; set; } = string.Empty;
        public decimal Discount { get; set; }
        public string DiscountType { get; set; } = string.Empty;
        public DateTime ValidTo { get; set; }
        public bool IsActive { get; set; }
        public int MaxUsage { get; set; }
        public int UsedCount { get; set; }
    }
}
