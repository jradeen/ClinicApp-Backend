using System;
using System.Net.Http.Headers;
using ClinicApp.API.Data;
using ClinicApp.API.Interfaces.IOrder;
using ClinicApp.API.Models;
using Microsoft.EntityFrameworkCore;

namespace ClinicApp.API.Repositories;

public class OrderRepository : IOrderRepository
{
    private readonly AppDbContext _context;
    public OrderRepository(AppDbContext context)
    {
        _context = context;
    }
    public async Task<List<Order>> CreateOrdersAsync(List<Order> orders)
    {
        await _context.Orders.AddRangeAsync(orders);
        await _context.SaveChangesAsync();

        return orders;
    }

    public async Task<List<Order>> GetByClinicIdAsync(int clinicId)
    {
        return await _context.Orders
            .Include(o => o.Items).ThenInclude(i => i.Product)
            .Include(o => o.User)
            .Include(o=>o.Clinic)
            .Where(o => o.ClinicId == clinicId)
            .AsNoTracking().ToListAsync();
    }

    public async Task<Order> GetByIdAsync(int id)
    {
        var order = await _context.Orders.Include(o => o.Clinic).FirstOrDefaultAsync(o => o.Id == id);
        if (order == null) return null;
        return order;
    }

    public async Task<List<Order>> GetByUserIdAsync(string userId)
    {
        return await _context.Orders
            .Include(o => o.Clinic)
            .Include(o=>o.User)
            .Include(o => o.Items).ThenInclude(i => i.Product)
            .Where(o => o.UserId == userId)
            .AsNoTracking().ToListAsync();
    }

    public async Task UpdateAsync(Order order)
    {
        _context.Orders.Update(order);
        await _context.SaveChangesAsync();
    }
}
