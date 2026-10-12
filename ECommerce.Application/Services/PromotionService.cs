using System;
using System.Collections.Generic;
using System.Text;
using ECommerce.Application.Interfaces;
using ECommerce.Domain.Entities;
using ECommerce.Domain.Enums;

namespace ECommerce.Application.Services
{
    public class PromotionService : IPromotionService
    {
        private readonly IRepository<Promotion> _promotions;

        public PromotionService(IRepository<Promotion> promotions)
        {
            _promotions = promotions;
        }

        public async Task<(Promotion?, decimal, string?)> ValidateAsync(string code, decimal subtotal)
        {
            var promo = await _promotions.GetOneAsync(p => p.Code == code.Trim().ToUpper());

            if (promo is null || !promo.IsActive) return (null, 0, "Invalid promotion code.");
            if (promo.ValidTo < DateTime.UtcNow) return (null, 0, "Promotion code has expired.");
            if (promo.UsedCount >= promo.MaxUsage) return (null, 0, "Promotion code usage limit reached.");

            var discount = promo.DiscountType == DiscountType.Percentage
                ? subtotal * promo.Discount / 100
                : promo.Discount;

            discount = Math.Min(discount, subtotal);  
            return (promo, Math.Round(discount, 2), null);
        }
    }
}
