using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace ECommerce.Application.DTOs.Cart.Requests
{
    public record AddToCartRequest
    {
        [Range(1, int.MaxValue)]
        public int ProductId { get; init; }
        [Range(1, 100)]
        public int Count { get; init; }
    }
}
