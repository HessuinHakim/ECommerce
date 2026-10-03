using System;
using System.Collections.Generic;
using System.Text;
using  ECommerce.Application.DTOs.Categories;
using ECommerce.Application.Interfaces;
using ECommerce.Domain.Entities;
using static ECommerce.Application.DTOs.Categories.CategoryDTO;

namespace ECommerce.Application.Services
{
    public class CategoryService : ICategoryService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IRepository<Category> _categoryRepository;
        public CategoryService(IUnitOfWork unitOfWork, IRepository<Category> categoryRepository)
        {
            _unitOfWork = unitOfWork;
            _categoryRepository = categoryRepository;
        }
        public async Task<IEnumerable<CategoryDTO>> GetAllAsync()
        {
            var categories = await _categoryRepository.GetAsync(c=> c.Status, tracked: false);
            return categories.Select(c=> new CategoryDTO
            {
                Id=c.Id,
                Name = c.Name,
                Description=c.Description,
                Status=c.Status
            });
        }
        public async Task<CategoryDTO?> GetByIdAsync(int id)
        {
            var categories = await _categoryRepository.GetOneAsync(c => c.Id == id, tracked: false);
            
            //if (categories == null) return null;

            return categories is null? null : new CategoryDTO
            {
                Id = categories.Id,
                Name = categories.Name,
                Description = categories.Description,
                Status = categories.Status
            };
        }
        public async Task<CategoryDTO> CreateAsync(CreateCategoryRequest createCategoryRequest)
        {

            var category = new Category
            {
                Name = createCategoryRequest.Name,
                Description = createCategoryRequest.Description,
                Status = true
            };

            await _categoryRepository.CreateAsync(category);
            await _unitOfWork.CommitAsync();

            return new CategoryDTO
            {
                Id = category.Id,
                Name = category.Name,
                Description = category.Description,
                Status = category.Status
            };

        }
        public async Task<bool> UpdateAsync(int id, UpdateCategoryRequest updateCategoryRequest)
        {
            var category = await _categoryRepository.GetOneAsync(c => c.Id == id);
            if (category is null) return false;

            category.Name = updateCategoryRequest.Name;
            category.Description = updateCategoryRequest.Description;
            category.Status = updateCategoryRequest.Status;
            _categoryRepository.Update(category);
            await _unitOfWork.CommitAsync();
            return true;
        }
        public async Task<bool> PatchAsync(int id, PatchCategoryRequest request)
        {
            var category = await _categoryRepository.GetOneAsync(c => c.Id == id);
            if (category is null) return false;

            if (!string.IsNullOrWhiteSpace(request.Name))
                category.Name = request.Name;

            if (request.Description is not null)
                category.Description = request.Description;

            if (request.Status.HasValue)
                category.Status = request.Status.Value;

            await _unitOfWork.CommitAsync();
            return true;
        }
        public async Task<bool> DeleteAsync(int id)
        {
            var category =await _categoryRepository.GetOneAsync(c => c.Id == id);
            if (category is null) return false;

            _categoryRepository.Delete(category);
            await _unitOfWork.CommitAsync();
            return true;
        }
    }
}
