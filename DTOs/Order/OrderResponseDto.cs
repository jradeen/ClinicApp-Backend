using System;

namespace ClinicApp.API.DTOs.Order;

public class OrderResponseDto
{
    public int Id { get; set; }
    public string CustomerPhone { get;  set; }
    public string CustomerEmail { get; set; }
    public string CustomerName { get; set; }
    public decimal TotalAmount { get; set; }
    public string ClinicName { get; set; }

    public string Status { get; set; }
    public DateTime CreatedAt { get; set; }
    public List<OrderItemResponseDto> Items { get; set; }
}

public class OrderItemResponseDto
{
    public int ProductId { get; set; }
    public string ProductName { get; set; }
    public int Quantity { get; set; }
    public decimal Price { get; set; }
}