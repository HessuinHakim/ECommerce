using System;
using System.Collections.Generic;
using System.Text;
using ECommerce.Application.DTOs.Promotions.Requests;
using ECommerce.Application.DTOs.Promotions.Responses;

namespace ECommerce.Application.Interfaces
{
    public interface IAdminPromotionService
    {
        Task<IEnumerable<PromotionResponse>> GetAllAsync();
        Task<(PromotionResponse? Promotion, string? Error)> CreateAsync(CreatePromotionRequest request);
        Task<bool> SetActiveAsync(int id, bool isActive);
    }
}
