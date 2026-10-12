using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;
using ECommerce.Application.DTOs.Cart.Requests;
using ECommerce.Application.DTOs.Cart.Responses;
using ECommerce.Application.Interfaces;
using ECommerce.Domain.Entities;

namespace ECommerce.Application.Services
{
    public class CartService : ICartService
    {
        private readonly IRepository<Cart> _carts;
        private readonly IRepository<Product> _products;
        private readonly IUnitOfWork _uow;

        public CartService(IRepository<Cart> carts, IRepository<Product> products, IUnitOfWork uow)
        {
            _carts = carts;
            _products = products;
            _uow = uow;
        }

        public async Task<(bool Succeeded, string? Error)> AddAsync(string userId, AddToCartRequest request)
        {
            var product = await _products.GetOneAsync(p => p.Id == request.ProductId, tracked: false);
            if(product is null) 
                return (false, "Product not found");
            var item = await _carts.GetOneAsync(x=>x.ApplicationUserId==userId && x.ProductId == request.ProductId);

            var newCount = (item? .Count ?? 0) + request.Count;
            if(newCount >product.Quantity)
                return (false, "Not enough stock");
            if(item is null)
            {
                await _carts.CreateAsync(new Cart
                {
                    ApplicationUserId = userId,
                    ProductId = request.ProductId,
                    Count = request.Count
                });

            }
            else
            {
                item.Count = newCount;
            }
            await _uow.CommitAsync();
            return (true, null);
        }

        public async Task ClearAsync(string userId)
        {
            var items = await _carts.GetAsync(c => c.ApplicationUserId == userId);
            foreach (var i in items) _carts.Delete(i);
            await _uow.CommitAsync();
        }

        public async Task<CartResponse> GetAsync(string userId)
        {
            var include = new Expression<Func<Cart,object?>>[] { c => c.Product };
            var items = await _carts.GetAsync(c => c.ApplicationUserId == userId, tracked: false, includes: include!);
            var response = new CartResponse();
            foreach(var p in items)
            {
                var unit = p.Product!.Price - (p.Product.Price * p.Product.Discount / 100);
                response.Items.Add(new CartItemResponse
                {
                    ProductId = p.ProductId,
                    Name = p.Product.Name,
                    MainImg = p.Product.MainImg,
                    UnitPrice = unit,
                    Count = p.Count,
                    LineTotal = unit * p.Count,
                    InStock = p.Product.Quantity >= p.Count
                }
                );

               
            }

            response.Total = response.Items.Sum(i=>i.LineTotal);
            
            return response;
        }

        public async Task<bool> RemoveAsync(string userId, int productId)
        {
            var item = await _carts.GetOneAsync(c => c.ApplicationUserId == userId && c.ProductId == productId);
            if (item is null) return false;

            _carts.Delete(item);
            await _uow.CommitAsync();
            return true;
        }

        public async Task<(bool Succeeded, string? Error)> UpdateCountAsync(string userId, int productId, int count)
        {
            var item = await _carts.GetOneAsync(c => c.ApplicationUserId == userId && c.ProductId == productId);
            if (item is null) return (false, "Item not in cart.");

            var product = await _products.GetOneAsync(p => p.Id == productId && p.Status && !p.IsDeleted, tracked: false);
            if (product is null) return (false, "Product not available.");

            if (count > product.Quantity) return (false, "Not enough stock.");

            item.Count = count;
            await _uow.CommitAsync();
            return (true, null);
        }
    }
}
