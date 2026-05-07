using System;
using ClinicApp.API.Models;

namespace ClinicApp.API.Interfaces.IOrder;

public interface IOrderRepository
{
    Task<List<Order>> CreateOrdersAsync(List<Order> orders);
    Task<Order> GetByIdAsync(int id);
    Task<List<Order>> GetByUserIdAsync(string userId);
    Task<List<Order>> GetByClinicIdAsync(int clinicId);
    Task UpdateAsync(Order order);
}