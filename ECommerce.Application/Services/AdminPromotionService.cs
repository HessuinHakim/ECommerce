using System;
using System.Collections.Generic;
using System.Text;
using ECommerce.Application.DTOs.Promotions.Requests;
using ECommerce.Application.DTOs.Promotions.Responses;
using ECommerce.Application.Interfaces;
using ECommerce.Domain.Entities;
using ECommerce.Domain.Enums;

namespace ECommerce.Application.Services
{
    public class AdminPromotionService : IAdminPromotionService
    {
        private readonly IRepository<Promotion> _promotions;
        private readonly IUnitOfWork _uow;

        public AdminPromotionService(IRepository<Promotion> promotions, IUnitOfWork uow)
        {
            _promotions = promotions;
            _uow = uow;
        }

        public async Task<IEnumerable<PromotionResponse>> GetAllAsync()
        {
            var list = await _promotions.GetAsync(tracked: false);
            return list.OrderByDescending(p => p.CreatedAt).Select(Map);
        }

        public async Task<(PromotionResponse?, string?)> CreateAsync(CreatePromotionRequest request)
        {
            var code = request.Code.Trim().ToUpper();

            if (await _promotions.GetOneAsync(p => p.Code == code, tracked: false) is not null)
                return (null, "Promotion code already exists.");

            if (request.ValidTo <= DateTime.UtcNow)
                return (null, "ValidTo must be in the future.");

            if (request.DiscountType == DiscountType.Percentage && request.Discount > 100)
                return (null, "Percentage discount cannot exceed 100.");

            var promo = new Promotion
            {
                Code = code,
                Discount = request.Discount,
                DiscountType = request.DiscountType,
                ValidTo = request.ValidTo.ToUniversalTime(),
                MaxUsage = request.MaxUsage
            };

            await _promotions.CreateAsync(promo);
            await _uow.CommitAsync();
            return (Map(promo), null);
        }

        public async Task<bool> SetActiveAsync(int id, bool isActive)
        {
            var promo = await _promotions.GetOneAsync(p => p.Id == id);
            if (promo is null) return false;

            promo.IsActive = isActive;
            await _uow.CommitAsync();
            return true;
        }

        private static PromotionResponse Map(Promotion p) => new()
        {
            Id = p.Id,
            Code = p.Code,
            Discount = p.Discount,
            DiscountType = p.DiscountType.ToString(),
            ValidTo = p.ValidTo,
            IsActive = p.IsActive,
            MaxUsage = p.MaxUsage,
            UsedCount = p.UsedCount
        };

    }
}
