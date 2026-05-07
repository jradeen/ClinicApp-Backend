using System;
using ClinicApp.API.DTOs.Order;
using ClinicApp.API.Interfaces.IClinic;
using ClinicApp.API.Interfaces.IOrder;
using ClinicApp.API.Interfaces.IProduct;
using ClinicApp.API.Models;

namespace ClinicApp.API.Services;

public class OrderService : IOrderService
{
    private readonly IOrderRepository _orderRepo;
    private readonly IProductRepository _productRepo;
    private readonly IClinicRepository _clinicRepo;

    public OrderService(IOrderRepository orderRepo, IProductRepository productRepo, IClinicRepository clinicRepo)
    {
        _orderRepo = orderRepo;
        _productRepo = productRepo;
        _clinicRepo = clinicRepo;
    }
    public async Task<List<OrderResponseDto>> CreateAsync(CreateOrderDto createOrderDto, string userId)
    {
        // 1. Get all product IDs from the request
        var productIds = createOrderDto.Items.Select(i => i.ProductId).ToList();


        var products = await _productRepo.GetListByIdsAsync(productIds);

        //to help look up which product belong to which clinic 
        var productLookup = products.ToDictionary(p => p.Id);

        //The GroupBy starts at the very first item in the Items list and asks: "Which clinic group does this belong to?"
        var itemsGroupedByClinic = createOrderDto.Items.GroupBy(item =>
         productLookup[item.ProductId].ClinicId);

        var createdOrders = new List<Order>();

        foreach (var group in itemsGroupedByClinic)
        {
            var order = new Order
            {
                UserId = userId,
                ClinicId = group.Key,
                CreatedAt = DateTime.UtcNow,
                Status = "Pending",
                Items = new List<OrderItem>()
            };

            decimal total = 0;

            foreach (var item in group)
            {
                // i get the entire product 
                var product = productLookup[item.ProductId];

                if (product.StockQuantity < item.Quantity)
                    throw new Exception($"Not enough in stock for {product.Name}");


                product.StockQuantity -= item.Quantity;//EF keep track and does the updates in the product table

                var orderItem = new OrderItem
                {
                    ProductId = product.Id,
                    ClinicId = product.ClinicId,
                    Quantity = item.Quantity,
                    Price = product.Price
                };

                total += product.Price * item.Quantity;
                order.Items.Add(orderItem);
            }

            order.TotalAmount = total;
            createdOrders.Add(order);
        }


        var result = await _orderRepo.CreateOrdersAsync(createdOrders);

        return result.Select(ToOrderResponseDto).ToList();
    }

    public async Task<List<OrderResponseDto>> GetUserOrdersAsync(string userId)
    {
        var orders = await _orderRepo.GetByUserIdAsync(userId);
        return orders.Select(ToOrderResponseDto).ToList();
    }

    public async Task<List<OrderResponseDto>> GetClinicOrdersAsync(int clinicId)
    {
        var orders = await _orderRepo.GetByClinicIdAsync(clinicId);
        return orders.Select(ToOrderResponseDto).ToList();
    }
    public async Task<bool> UpdateStatusAsync(int orderId, string newStatus, string ownerId)
    {
        var validStatuses = new[] { "Confirmed", "Cancelled", "Delivered" };
        if (!validStatuses.Contains(newStatus)) throw new Exception("Invalid status name.{Confirmed, Cancelled, Delivered}");

        var order = await _orderRepo.GetByIdAsync(orderId);
        if (order == null) return false;

        if (order.Clinic.OwnerId != ownerId)
            throw new UnauthorizedAccessException("You don't have permission to change this order status.");

        order.Status = newStatus;
        await _orderRepo.UpdateAsync(order);
        return true;


    }

    private OrderResponseDto ToOrderResponseDto(Order order)
    {
        return new OrderResponseDto
        {
            Id = order.Id,
            CustomerEmail = order.User?.Email ?? "No Email",
            CustomerName = order.User?.UserName ?? "Deleted User",
            CustomerPhone = order.User?.PhoneNumber ?? "No Phone Number",
            TotalAmount = order.TotalAmount,
            Status = order.Status,
            CreatedAt = order.CreatedAt,
            ClinicName = order.Clinic?.Name ?? "Clinic name Unavailable",
            Items = order.Items.Select(i => new OrderItemResponseDto
            {
                ProductId = i.ProductId,
                ProductName = i.Product?.Name ?? "Product name Unavailable",
                Quantity = i.Quantity,
                Price = i.Price
            }).ToList()
        };
    }

}

