using System;
using System.Collections.Generic;
using System.Text;
using ECommerce.Application.DTOs.Cart.Requests;
using ECommerce.Application.DTOs.Cart.Responses;

namespace ECommerce.Application.Interfaces
{
    public interface ICartService
    {
        Task<CartResponse> GetAsync(string userId);
        Task<(bool Succeeded, string? Error)> AddAsync(string userId, AddToCartRequest request);
        Task<(bool Succeeded, string? Error)> UpdateCountAsync(string userId, int productId, int count);
        Task<bool> RemoveAsync(string userId, int productId);
        Task ClearAsync(string userId);
    }
}
