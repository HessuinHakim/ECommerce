using System;
using System.Collections.Generic;
using System.Text;
using ECommerce.Domain.Entities;

namespace ECommerce.Application.Interfaces
{
    public interface IPromotionService
    {
        Task<(Promotion? Promotion, decimal Discount, string? Error)> ValidateAsync(string code, decimal subtotal);

    }
}
