using System;
using ClinicApp.API.DTOs.Product;

namespace ClinicApp.API.Interfaces.IProduct;

public interface IProductService
{
    Task<ProductResponseDto> CreateAsync(CreateProductDto createProductDto, string ownerId);
    Task<List<ProductResponseDto>> GetByClinicAsync(int clinicId);
}
