using System;

namespace ClinicApp.API.DTOs.Product;

public class ProductResponseDto
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public decimal Price { get; set; }
    public int StockQuantity { get; set; }
    public string ClinicName { get; set; }
    public int ClinicId { get; set; }
    public List<int> TagIds { get; set; } = new();

    public List<string> Tags { get; set; } = new();

    public string ImageUrl { get; set; } = string.Empty;

}
