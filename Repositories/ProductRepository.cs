using System;
using ClinicApp.API.Data;
using ClinicApp.API.Interfaces.IProduct;
using ClinicApp.API.Models;
using Microsoft.EntityFrameworkCore;

namespace ClinicApp.API.Repositories;

public class ProductRepository : IProductRepository
{
    private readonly AppDbContext _context;
    public ProductRepository(AppDbContext context)
    {
        _context = context;
    }
    public async Task<Product> CreateAsync(Product product)
    {
        await _context.Products.AddAsync(product);
        await _context.SaveChangesAsync();
        return product;
    }

    public async Task<List<Product>> GetByClinicIdAsync(int clinicId)
    {
        return await _context.Products.Where(p => p.ClinicId == clinicId).ToListAsync();
    }

    public async Task<Product?> GetByIdAsync(int id)
    {
        return await _context.Products.FindAsync(id);
    }

    public async Task<List<Product>> GetListByIdsAsync(List<int> ids)
    {
        return await _context.Products
            .Where(p => ids.Contains(p.Id))
            .ToListAsync();
    }
}
