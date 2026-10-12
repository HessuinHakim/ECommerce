using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;
using ECommerce.Application.DTOs.Files;
using ECommerce.Application.DTOs.Pagination;
using ECommerce.Application.DTOs.Products.Requests;
using ECommerce.Application.DTOs.Products.Responses;
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

        public async Task<PageResultDTO<ProductListResponse>> GetAllAsync(ProductQueryParamsDTO q)
        {
            var search = q.Search?.Trim();

            Expression<Func<Product, bool>> filter = p =>
                !p.IsDeleted && p.Status
                && (search == null || p.Name.Contains(search))
                && (q.CategoryId == null || p.CategoryId == q.CategoryId)
                && (q.BrandId == null || p.BrandId == q.BrandId)
                && (q.MinPrice == null || p.Price >= q.MinPrice)
                && (q.MaxPrice == null || p.Price <= q.MaxPrice);

            Func<IQueryable<Product>, IOrderedQueryable<Product>> orderBy = q.SortBy switch
            {
                "price" => x => x.OrderBy(p => p.Price).ThenBy(p => p.Id),
                "price_desc" => x => x.OrderByDescending(p => p.Price).ThenBy(p => p.Id),
                "name" => x => x.OrderBy(p => p.Name).ThenBy(p => p.Id),
                _ => x => x.OrderByDescending(p => p.Id)   
            };

            var includes = new Expression<Func<Product, object>>[] { p => p.Category, p => p.Brand };

            var (items, total) = await _productRepository.GetPagedAsync(
                filter, q.Page, q.PageSize, tracked: false, includes, orderBy);

            return new PageResultDTO<ProductListResponse>
            {
                Page = q.Page,
                PageSize = q.PageSize,
                TotalCount = total,
                Items = items.Select(p => new ProductListResponse
                {
                    Id = p.Id,
                    Name = p.Name,
                    MainImg = p.MainImg,
                    Price = p.Price,
                    Discount = p.Discount,
                    FinalPrice = p.Price - (p.Price * p.Discount / 100),
                    Rate = p.Rate,
                    Status = p.Status,
                    CategoryName = p.Category.Name,
                    BrandName = p.Brand.Name
                }).ToList()
            };
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
