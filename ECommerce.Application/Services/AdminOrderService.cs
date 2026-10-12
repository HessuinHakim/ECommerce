using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;
using ECommerce.Application.DTOs.Orders.Responses;
using ECommerce.Application.DTOs.Pagination;
using ECommerce.Application.Interfaces;
using ECommerce.Domain.Entities;
using ECommerce.Domain.Enums;

namespace ECommerce.Application.Services
{
    public class AdminOrderService : IAdminOrderService
    {
        private readonly IRepository<Order> _orders;
        private readonly IRepository<Product> _products;

        private readonly IUnitOfWork _uow;
        private static readonly Dictionary<OrderStatus, OrderStatus[]> AllowedTransitions = new()
        {
            [OrderStatus.Pending] = new[] { OrderStatus.Cancelled },
            [OrderStatus.Paid] = new[] { OrderStatus.Shipped, OrderStatus.Cancelled },
            [OrderStatus.Shipped] = new[] { OrderStatus.Delivered },
            [OrderStatus.Delivered] = Array.Empty<OrderStatus>(),
            [OrderStatus.Cancelled] = Array.Empty<OrderStatus>()
        };

        public AdminOrderService(IRepository<Order> orders, IRepository<Product> products, IUnitOfWork uow)
        {
            _orders = orders;
            _products = products;
            _uow = uow;
        }

        public async Task<PageResultDTO<AdminOrderResponse>> GetAllAsync(OrderStatus? status, int page, int pageSize)
        {
            var (items, total) = await _orders.GetPagedAsync(
                o => status == null || o.Status == status,
                page,
                pageSize,
                tracked: false,
                includes: new Expression<Func<Order, object>>[] { o => o.Items },
                orderBy: q => q.OrderByDescending(o => o.CreatedAt));

            return new PageResultDTO<AdminOrderResponse>
            {
                Page = page,
                PageSize = pageSize,
                TotalCount = total,
                Items = items.Select(o => new AdminOrderResponse
                {
                    Id = o.Id,
                    ApplicationUserId = o.ApplicationUserId,
                    Status = o.Status.ToString(),
                    CreatedAt = o.CreatedAt,
                    Subtotal = o.Subtotal,
                    DiscountAmount = o.DiscountAmount,
                    Total = o.Total,
                    Items = o.Items.Select(i => new OrderItemResponse(
                        i.ProductId, i.ProductName, i.UnitPrice, i.Count, i.UnitPrice * i.Count)).ToList()
                }).ToList()
            };
        }
        public async Task<(bool, string?)> UpdateStatusAsync(int orderId, OrderStatus newStatus)
        {
            var order = await _orders.GetOneAsync(
                o => o.Id == orderId,
                includes: new Expression<Func<Order, object>>[] { o => o.Items });

            if (order is null) return (false, "Order not found.");

            if (!AllowedTransitions[order.Status].Contains(newStatus))
                return (false, $"Cannot change status from {order.Status} to {newStatus}.");

            if (newStatus == OrderStatus.Cancelled)
            {
                foreach (var item in order.Items)
                {
                    var product = await _products.GetOneAsync(p => p.Id == item.ProductId);
                    if (product is not null) product.Quantity += item.Count;
                }
            }

            order.Status = newStatus;
            await _uow.CommitAsync();
            return (true, null);
        }
    }
}
