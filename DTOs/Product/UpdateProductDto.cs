using System;
using System.ComponentModel.DataAnnotations;

namespace ClinicApp.API.DTOs.Product;

public class UpdateProductDto
{
    [Required, StringLength(100)]
    public string Name { get; set; } = string.Empty;

    [Required, MaxLength(500)]
    public string Description { get; set; } = string.Empty;

    [Range(0.1, 10000)]
    public decimal Price { get; set; }

    [Range(0, 10000)]
    public int StockQuantity { get; set; }
    public List<int> TagIds { get; set; } = new();

    public string? ImageUrl { get; set; }
}
