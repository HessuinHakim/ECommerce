using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using ECommerce.Domain.Enums;

namespace ECommerce.Application.DTOs.Orders.Requests
{
    public record UpdateOrderStatusRequest
    {
        [Required]
        public OrderStatus Status { get; set; }
    }
}
