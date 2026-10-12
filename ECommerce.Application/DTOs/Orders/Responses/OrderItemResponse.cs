using System;
using System.Collections.Generic;
using System.Text;

namespace ECommerce.Application.DTOs.Orders.Responses
{
    public record OrderItemResponse(int ProductId, string ProductName, decimal UnitPrice, int Count, decimal LineTotal);
    
    
}
