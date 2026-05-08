using System;
using ClinicApp.API.DTOs.Product;

namespace ClinicApp.API.Interfaces.IProduct;

public interface IProductService
{
    Task<ProductResponseDto> CreateAsync(CreateProductDto createProductDto, string ownerId);
    Task<List<ProductResponseDto>> GetByClinicAsync(int clinicId);
    Task<bool> DeleteProductAsync(int productId,string ownerId);
    Task<ProductResponseDto> UpdateProductAsync(int productId, UpdateProductDto updateDto, string ownerId);
    Task<List<ProductResponseDto>> GetAllAsync();
}
