using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;
using ECommerce.Application.DTOs.Orders.Requests;
using ECommerce.Application.DTOs.Orders.Responses;
using ECommerce.Application.Exceptions;
using ECommerce.Application.Interfaces;
using ECommerce.Domain.Entities;

namespace ECommerce.Application.Services
{
    public class OrderService : IOrderService
    {
        private readonly IRepository<Cart> _carts;
        private readonly IRepository<Product> _products;
        private readonly IRepository<Order> _orders;
        private readonly IUnitOfWork _uow;

        public OrderService(IRepository<Cart> carts, IRepository<Product> products,
                            IRepository<Order> orders, IUnitOfWork uow)
        {
            _carts = carts; _products = products; _orders = orders; _uow = uow;
        }

        public async Task<(OrderResponse?, string?)> CheckoutAsync(string userId, CheckoutRequest request)
        {
            await _uow.BeginTransactionAsync();
            try
            {
                var cartItems = (await _carts.GetAsync(
                    c => c.ApplicationUserId == userId,
                    includes: new Expression<Func<Cart, object>>[] { c => c.Product! })).ToList();

                if (cartItems.Count == 0)
                {
                    await _uow.RollbackTransactionAsync();
                    return (null, "Cart is empty.");
                }

                var order = new Order { ApplicationUserId = userId };

                foreach (var item in cartItems)
                {
                    var p = item.Product;

                    if (p.IsDeleted || !p.Status)
                    {
                        await _uow.RollbackTransactionAsync();
                        return (null, $"'{p.Name}' is no longer available.");
                    }
                    if (p.Quantity < item.Count)
                    {
                        await _uow.RollbackTransactionAsync();
                        return (null, $"Not enough stock for '{p.Name}'.");
                    }

                    var unitPrice = p.Price - (p.Price * p.Discount / 100);

                    order.Items.Add(new OrderItem
                    {
                        ProductId = p.Id,
                        ProductName = p.Name,
                        UnitPrice = unitPrice,
                        Count = item.Count
                    });

                    p.Quantity -= item.Count;
                }

                order.Subtotal = order.Items.Sum(i => i.UnitPrice * i.Count);
                order.Total = order.Subtotal - order.DiscountAmount; 

                await _orders.CreateAsync(order);

                foreach (var item in cartItems) _carts.Delete(item);

                await _uow.CommitTransactionAsync();
                return (Map(order), null);
            }
            catch (ConcurrencyException)
            {
                await _uow.RollbackTransactionAsync();
                return (null, "Stock changed while processing your order. Please try again.");
            }
            catch
            {
                await _uow.RollbackTransactionAsync();
                throw;
            }
        }

        public async Task<IEnumerable<OrderResponse>> GetMyOrdersAsync(string userId)
        {
            var orders = await _orders.GetAsync(
                o => o.ApplicationUserId == userId, tracked: false,
                includes: new Expression<Func<Order, object>>[] { o => o.Items });
            return orders.OrderByDescending(o => o.CreatedAt).Select(Map);
        }

        public async Task<OrderResponse?> GetMyOrderAsync(string userId, int orderId)
        {
            var order = await _orders.GetOneAsync(
                o => o.Id == orderId && o.ApplicationUserId == userId, tracked: false,
                includes: new Expression<Func<Order, object>>[] { o => o.Items });
            return order is null ? null : Map(order);
        }

        private static OrderResponse Map(Order o) => new()
        {
            Id = o.Id,
            Status = o.Status.ToString(),
            CreatedAt = o.CreatedAt,
            Subtotal = o.Subtotal,
            DiscountAmount = o.DiscountAmount,
            Total = o.Total,
            Items = o.Items.Select(i =>
                new OrderItemResponse(i.ProductId, i.ProductName, i.UnitPrice, i.Count, i.UnitPrice * i.Count)).ToList()
        };
    }
}
