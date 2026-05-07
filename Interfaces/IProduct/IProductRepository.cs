using System;
using ClinicApp.API.Models;

namespace ClinicApp.API.Interfaces.IProduct;

public interface IProductRepository
{
    Task<Product> CreateAsync(Product product);
    Task<List<Product>> GetByClinicIdAsync(int clinicId);
    Task<Product?> GetByIdAsync(int id);
    Task<List<Product>> GetListByIdsAsync(List<int> ids);
}
