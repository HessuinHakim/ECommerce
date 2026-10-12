using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace ECommerce.Application.DTOs.Orders.Requests
{
    public record CheckoutRequest
    {
        [MaxLength(50)]
        public string? PromotionCode { get; set; }
    }
}
