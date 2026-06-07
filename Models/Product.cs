using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ClinicApp.API.Models;

public class Product
{
    public int Id { get; set; }

    [Required]
    public string Name { get; set; }

    public string Description { get; set; }

    public decimal Price { get; set; }

    public int StockQuantity { get; set; }

    public int ClinicId { get; set; }

    [ForeignKey("ClinicId")]
    public Clinic Clinic { get; set; }
    public ICollection<ProductTag> ProductTags { get; set; } = new List<ProductTag>();

    public string ImageUrl { get; set; } = "uploads/placeholders/default.jpg";

}
