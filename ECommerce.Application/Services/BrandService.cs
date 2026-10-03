using System;
using System.Collections.Generic;
using System.Text;
using ECommerce.Application.DTOs.Brands;
using ECommerce.Application.Interfaces;
using ECommerce.Domain.Entities;

namespace ECommerce.Application.Services
{
    public class BrandService : IBrandService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IRepository<Brand> _brandRepository;
        public BrandService(IUnitOfWork unitOfWork, IRepository<Brand> brandRepository)
        {
            _unitOfWork = unitOfWork;
            _brandRepository = brandRepository;
        }
        public async Task<IEnumerable<BrandDTO>> GetAllAsync()
        {
            var brands = await _brandRepository.GetAsync(b => b.Status, tracked: false);
            return brands.Select(b => new BrandDTO
            {
                Id = b.Id,
                Name = b.Name,
                Description = b.Description,
                Status = b.Status
            });
        }
        public async Task<BrandDTO?> GetByIdAsync(int id)
        {
            var brands = await _brandRepository.GetOneAsync(b => b.Id == id, tracked: false);

            //if (brands == null) return null;

            return brands is null ? null : new BrandDTO
            {
                Id = brands.Id,
                Name = brands.Name,
                Description = brands.Description,
                Status = brands.Status
            };
        }
        public async Task<BrandDTO> CreateAsync(CreateBrandRequest createBrandRequest)
        {

            var brand = new Brand
            {
                Name = createBrandRequest.Name,
                Description = createBrandRequest.Description,
                Status = true
            };

            await _brandRepository.CreateAsync(brand);
            await _unitOfWork.CommitAsync();

            return new BrandDTO
            {
                Id = brand.Id,
                Name = brand.Name,
                Description = brand.Description,
                Status = brand.Status
            };

        }
        public async Task<bool> UpdateAsync(int id, UpdateBrandRequest updateBrandRequest)
        {
            var brand = await _brandRepository.GetOneAsync(b => b.Id == id);
            if (brand is null) return false;

            brand.Name = updateBrandRequest.Name;
            brand.Description = updateBrandRequest.Description;
            brand.Status = updateBrandRequest.Status;
            _brandRepository.Update(brand);
            await _unitOfWork.CommitAsync();
            return true;
        }
        public async Task<bool> PatchAsync(int id, PatchBrandRequest request)
        {
            var brand = await _brandRepository.GetOneAsync(b => b.Id == id);
            if (brand is null) return false;

            if (!string.IsNullOrWhiteSpace(request.Name))
                brand.Name = request.Name;

            if (request.Description is not null)
                brand.Description = request.Description;

            if (request.Status.HasValue)
                brand.Status = request.Status.Value;

            await _unitOfWork.CommitAsync();
            return true;
        }
        public async Task<bool> DeleteAsync(int id)
        {
            var brand = await _brandRepository.GetOneAsync(b => b.Id == id);
            if (brand is null) return false;

            _brandRepository.Delete(brand);
            await _unitOfWork.CommitAsync();
            return true;
        }
    }
}
