using System;
using System.Collections.Generic;
using System.Text;
using ECommerce.Application.DTOs.Brands;
using  ECommerce.Application.DTOs.Categories;

namespace ECommerce.Application.Interfaces
{
    public interface ICategoryService

    {
        Task<IEnumerable<CategoryDTO>> GetAllAsync();
        Task<CategoryDTO?> GetByIdAsync(int id);

        Task<CategoryDTO> CreateAsync(CreateCategoryRequest createCategoryRequest);
        Task<bool> UpdateAsync(int id, UpdateCategoryRequest updateCategoryRequest);
        Task<bool> PatchAsync(int id, PatchCategoryRequest patchCategoryRequest);
        Task<bool> DeleteAsync(int id);
    }
}
