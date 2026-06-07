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

    public async Task<List<Product>> GetAllAsync()
    {
        return await _context.Products
            .Include(p => p.Clinic)
            .Include(p => p.ProductTags)
            .ThenInclude(pt => pt.Tag)
            .ToListAsync();
    }

    public async Task<List<Product>> GetByClinicIdAsync(int clinicId)
    {
        return await _context.Products
            .Include(p => p.ProductTags)
            .ThenInclude(pt => pt.Tag)
            .Where(p => p.ClinicId == clinicId).ToListAsync();
    }

    public async Task<Product?> GetByIdAsync(int id)
    {
        return await _context.Products
        .Include(p => p.Clinic)
        .Include(p => p.ProductTags)
        .ThenInclude(pt => pt.Tag)
        .FirstOrDefaultAsync(p => p.Id == id);
    }

    public async Task<List<Product>> GetListByIdsAsync(List<int> ids)
    {
        return await _context.Products
            .Include(p => p.ProductTags)
            .ThenInclude(pt => pt.Tag)
            .Where(p => ids.Contains(p.Id))
            .ToListAsync();
    }
    public async Task UpdateAsync(Product product)
    {
        _context.Products.Update(product);
        await _context.SaveChangesAsync();

    }
    public async Task<Product?> DeleteAsync(int id)
    {
        var product = await _context.Products.FirstOrDefaultAsync(p => p.Id == id);
        if (product == null) return null;
        _context.Products.Remove(product);
        await _context.SaveChangesAsync();
        return product;

    }
}
