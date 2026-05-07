using System;
using System.ComponentModel.DataAnnotations;

namespace ClinicApp.API.DTOs.Order;

public class CreateOrderDto
{
    [Required, MinLength(1, ErrorMessage = "Your cart cannot be empty")]
    public List<OrderItemDto> Items { get; set; }
}

public class OrderItemDto
{
    public int ProductId { get; set; }

    [Range(1, int.MaxValue, ErrorMessage = "Amount must be at least 1")]
    [System.ComponentModel.DefaultValue(1)] 
    public int Quantity { get; set; }
}

