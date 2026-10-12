using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;
using ECommerce.Application.DTOs.Files;
using ECommerce.Application.DTOs.Products;
using ECommerce.Application.Interfaces;
using ECommerce.Domain.Entities;

namespace ECommerce.Application.Services
{
    public class ProductService : IProductService
    {

        private readonly IImageService _imageService;
        private readonly IRepository<Product> _productRepository;
        private readonly IUnitOfWork _unitOfWork;

        public ProductService(IImageService imageService, IRepository<Product> productRepository, IUnitOfWork unitOfWork)
        {
            _imageService = imageService;
            _productRepository = productRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<IEnumerable<ProductListResponse>> GetAllAsync()


        {

            var include = new Expression<Func<Product,object>> []
            {
                x => x.Category,
                x => x.Brand
            }
            ;
            var products  = await _productRepository.GetAsync(x=> !x.IsDeleted,tracked:false,includes:include);

            return products.Select(p => new ProductListResponse
            {
                Id = p.Id,
                Name = p.Name,
                Price = p.Price,
                MainImg=p.MainImg,
                Discount = p.Discount,
                FinalPrice = p.Price - (p.Price * p.Discount / 100),
                Rate = p.Rate,
                Status = p.Status,
                CategoryName = p.Category.Name,
                BrandName = p.Brand.Name,

                
            });
        }

        public async Task<ProductDetailsResponse?> GetAsync( int id) 
        {
            var include = new Expression<Func<Product, object>>[]
         {
                x => x.Category,
                x => x.Brand
         };
            var product = await _productRepository.GetOneAsync(p=> p.Id == id && !p.IsDeleted, tracked: false, includes: include);
            if (product is null) return null;

            return new ProductDetailsResponse
            {
                Id = product.Id,
                Name = product.Name,
                Price = product.Price,
                MainImg = product.MainImg,
                Discount = product.Discount,
                FinalPrice = product.Price - (product.Price * product.Discount / 100),
                Rate = product.Rate,
                CategoryName = product.Category.Name,
                BrandName = product.Brand.Name,
                Description = product.Description
            };
        }

        public async Task<ProductDetailsResponse> CreateAsync(CreateProductRequest createProductRequest, FileUpload mainImg)
        {
            var img = await _imageService.SaveAsync(mainImg, "products");
            var product = new Product
            {
                Name = createProductRequest.Name,
                Description = createProductRequest.Description,
                Price = createProductRequest.Price,
                Discount = createProductRequest.Discount,
                Quantity = createProductRequest.Quantity,
                CategoryId = createProductRequest.CategoryId,
                BrandId = createProductRequest.BrandId,
                MainImg = img

            };

            try
            {
                 await _productRepository.CreateAsync(product);
                 await _unitOfWork.CommitAsync();
            }
            catch
            {
                _imageService.Delete(img,"products");
                throw;
            }
           


            return (await GetAsync(product.Id))!;

        }

            public async Task<bool> UpdateAsync(int id, UpdateProductRequest updateProductRequest, FileUpload? mainImg = null)
        {
            var product = await _productRepository.GetOneAsync(p => p.Id == id && !p.IsDeleted);
            if (product is null) return false;

            if (!string.IsNullOrWhiteSpace(updateProductRequest.Name)) product.Name = updateProductRequest.Name;
            if (updateProductRequest.Description is not null) product.Description = updateProductRequest.Description;
            if (updateProductRequest.Price.HasValue) product.Price = updateProductRequest.Price.Value;
            if (updateProductRequest.Discount.HasValue) product.Discount = updateProductRequest.Discount.Value;
            if (updateProductRequest.Quantity.HasValue) product.Quantity = updateProductRequest.Quantity.Value;
            if (updateProductRequest.Status.HasValue) product.Status = updateProductRequest.Status.Value;
            if (updateProductRequest.CategoryId.HasValue) product.CategoryId = updateProductRequest.CategoryId.Value;
            if (updateProductRequest.BrandId.HasValue) product.BrandId = updateProductRequest.BrandId.Value;

            await _unitOfWork.CommitAsync();
            return true;
          

        }

        public async Task<bool> DeleteAsync(int id)
        {
            var product = await _productRepository.GetOneAsync(p => p.Id == id && !p.IsDeleted);
            if (product is null) return false;
            product.IsDeleted = true;
            _productRepository.Update(product);
            await _unitOfWork.CommitAsync();
            return true;
        }

    }
}
