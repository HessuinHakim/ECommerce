using System;
using System.Collections.Generic;
using System.Text;
using ECommerce.Application.DTOs.Orders.Responses;
using ECommerce.Application.DTOs.Pagination;
using ECommerce.Domain.Enums;

namespace ECommerce.Application.Interfaces
{
    public interface IAdminOrderService
    {
        Task<PageResultDTO<AdminOrderResponse>> GetAllAsync(OrderStatus? status, int page, int pageSize);
        Task<(bool Succeeded, string? Error)> UpdateStatusAsync(int orderId, OrderStatus newStatus);
    }
}
