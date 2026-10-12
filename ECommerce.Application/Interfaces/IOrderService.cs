using System;
using System.Collections.Generic;
using System.Text;
using ECommerce.Application.DTOs.Orders.Requests;
using ECommerce.Application.DTOs.Orders.Responses;

namespace ECommerce.Application.Interfaces
{
    public interface IOrderService

    {
        Task<(OrderResponse? Order, string? Error)> CheckoutAsync(string userId, CheckoutRequest request);
        Task<IEnumerable<OrderResponse>> GetMyOrdersAsync(string userId);
        Task<OrderResponse?> GetMyOrderAsync(string userId, int orderId);
    }
}
