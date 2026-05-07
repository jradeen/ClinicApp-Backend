using System;
using ClinicApp.API.DTOs.Order;

namespace ClinicApp.API.Interfaces.IOrder;

public interface IOrderService
{
    Task<List<OrderResponseDto>> CreateAsync(CreateOrderDto createOrderDto, string userId);
    Task<List<OrderResponseDto>> GetUserOrdersAsync(string userId);
    Task<List<OrderResponseDto>> GetClinicOrdersAsync(int clinicId);
    Task<bool> UpdateStatusAsync(int orderId, string newStatus, string ownerId);

}
