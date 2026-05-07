using System;
using ClinicApp.API.DTOs.Product;
using ClinicApp.API.Interfaces.IClinic;
using ClinicApp.API.Interfaces.IProduct;
using ClinicApp.API.Models;

namespace ClinicApp.API.Services;

public class ProductService : IProductService
{
    private readonly IProductRepository _productRepo;
    private readonly IClinicRepository _clinicRepo;
    public ProductService(IProductRepository productRepo, IClinicRepository clinicRepo)
    {
        _clinicRepo = clinicRepo;
        _productRepo = productRepo;
    }

    public async Task<ProductResponseDto> CreateAsync(CreateProductDto createProductDto, string ownerId)
    {
        var clinic = await _clinicRepo.GetByOwnerIdAsync(ownerId);
        if (clinic == null) return null;

        var product=new Product
        {
          Name=createProductDto.Name,
          Description=createProductDto.Description,
          Price=createProductDto.Price,
          StockQuantity=createProductDto.StockQuantity,
          ClinicId=clinic.Id,

        };
        var result =await _productRepo.CreateAsync(product);

        return ToProductResponseDto(result);

    }

    public async Task<List<ProductResponseDto>> GetByClinicAsync(int clinicId)
    {
      
        var products=await _productRepo.GetByClinicIdAsync(clinicId);

        return products.Select(ToProductResponseDto).ToList();
    }

    private ProductResponseDto ToProductResponseDto(Product product)
    {
        return new ProductResponseDto{
            Id=product.Id,
            Name=product.Name,
            Description=product.Description,
            Price=product.Price,
            StockQuantity=product.StockQuantity,
            ClinicId=product.ClinicId,
            
        };
    }
}
