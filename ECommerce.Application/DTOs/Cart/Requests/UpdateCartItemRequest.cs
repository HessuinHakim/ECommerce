using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace ECommerce.Application.DTOs.Cart.Requests
{
    public record UpdateCartItemRequest
    {
        [Range(1, 100)]
        public int Count { get; init; }
    }
}
