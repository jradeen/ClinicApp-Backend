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
    private readonly IWebHostEnvironment _env;

    public ProductService(IWebHostEnvironment environment, IProductRepository productRepo, IClinicRepository clinicRepo)
    {
        _clinicRepo = clinicRepo;
        _productRepo = productRepo;
        _env = environment;
    }

    public async Task<ProductResponseDto> CreateAsync(CreateProductDto createProductDto, string ownerId)
    {
        var clinic = await _clinicRepo.GetByOwnerIdAsync(ownerId);
        if (clinic == null) return null;

        var product = new Product
        {
            Name = createProductDto.Name,
            Description = createProductDto.Description,
            Price = createProductDto.Price,
            StockQuantity = createProductDto.StockQuantity,
            ClinicId = clinic.Id,
            ImageUrl = !string.IsNullOrEmpty(createProductDto.ImageUrl) ? createProductDto.ImageUrl : ""

        };
        var result = await _productRepo.CreateAsync(product);

        return ToProductResponseDto(result);

    }


    public async Task<List<ProductResponseDto>> GetAllAsync()
    {
        var products = await _productRepo.GetAllAsync();
        return products.Select(ToProductResponseDto).ToList();
    }

    public async Task<List<ProductResponseDto>> GetByClinicAsync(int clinicId)
    {

        var products = await _productRepo.GetByClinicIdAsync(clinicId);

        return products.Select(ToProductResponseDto).ToList();
    }

    public async Task<bool> DeleteProductAsync(int productId, string ownerId)
    {
        var product = await _productRepo.GetByIdAsync(productId);
        if (product == null)
            return false;
        if (product.Clinic.OwnerId != ownerId)
            throw new UnauthorizedAccessException("You don't have permission to delete this product");

        await _productRepo.DeleteAsync(productId);
        return true;
    }
    public async Task<ProductResponseDto> UpdateProductAsync(int productId, UpdateProductDto updateDto, string ownerId)
    {
        var product = await _productRepo.GetByIdAsync(productId);
        if (product == null)
            return null;
        if (product.Clinic.OwnerId != ownerId)
            throw new UnauthorizedAccessException("You don't have permission to alter this product");

        product.Name = updateDto.Name;
        product.Description = updateDto.Description;
        product.Price = updateDto.Price;
        product.StockQuantity = updateDto.StockQuantity;
        product.ProductTags.Clear();

         if (updateDto.TagIds != null)
        {
            foreach (var tagId in updateDto.TagIds)
            {
                product.ProductTags.Add(new ProductTag {ProductId = product.Id,TagId = tagId});
            }
        }

        if (!string.IsNullOrEmpty(updateDto.ImageUrl) && product.ImageUrl != updateDto.ImageUrl)
        {
            var oldFilePath = Path.Combine(_env.WebRootPath, product.ImageUrl);

            if (System.IO.File.Exists(oldFilePath))
            {
                System.IO.File.Delete(oldFilePath);
            }
            product.ImageUrl = updateDto.ImageUrl;
        }

        await _productRepo.UpdateAsync(product);
        return ToProductResponseDto(product);
    }

    private ProductResponseDto ToProductResponseDto(Product product)
    {
        return new ProductResponseDto
        {
            Id = product.Id,
            Name = product.Name,
            Description = product.Description,
            Price = product.Price,
            StockQuantity = product.StockQuantity,
            ClinicName = product.Clinic?.Name ?? "Clinic Name unavailable",
            ClinicId = product.ClinicId,
            Tags = product.ProductTags.Select(pt => pt.Tag?.Name ?? "Unknown").ToList(),
            ImageUrl = product.ImageUrl,
        };
    }
}
