using System;
using System.Collections.Generic;
using System.Text;
using ECommerce.Application.DTOs.Brands;

namespace ECommerce.Application.Interfaces
{
    public interface IBrandService
    {
        Task<IEnumerable<BrandDTO>> GetAllAsync();
        Task<BrandDTO?> GetByIdAsync(int id);

        Task<BrandDTO> CreateAsync(CreateBrandRequest createBrandRequest);
        Task<bool> UpdateAsync(int id, UpdateBrandRequest updateBrandRequest);
        Task<bool> PatchAsync(int id, PatchBrandRequest patchBrandRequest);
        Task<bool> DeleteAsync(int id);
    }
}
