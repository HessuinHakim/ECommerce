using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using ECommerce.Domain.Enums;

namespace ECommerce.Application.DTOs.Promotions.Requests
{
    public record CreatePromotionRequest
    {
        [Required, StringLength(50, MinimumLength = 3)]
        public string Code { get; set; } = string.Empty;

        [Range(0.01, 1_000_000)]
        public decimal Discount { get; set; }

        public DiscountType DiscountType { get; set; } = DiscountType.Percentage;

        [Required]
        public DateTime ValidTo { get; set; }

        [Range(1, int.MaxValue)]
        public int MaxUsage { get; set; } = 100;
    }
}
