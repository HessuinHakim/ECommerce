using System;
using System.Collections.Generic;
using System.Text;
using ECommerce.Application.DTOs.Files;
using ECommerce.Application.DTOs.Products;

namespace ECommerce.Application.Interfaces
{
    public interface IProductService
    {
        Task <IEnumerable<ProductListResponse>> GetAllAsync();
        Task<ProductDetailsResponse?> GetAsync(int id);
        Task<ProductDetailsResponse> CreateAsync(CreateProductRequest createProductRequest, FileUpload mainImg);
        Task<bool> UpdateAsync(int id, UpdateProductRequest updateProductRequest, FileUpload? mainImg);
        Task<bool> DeleteAsync(int id);


    }
}
