using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace ClinicApp.API.Models;

public class Order
{
    public int Id { get; set; }

    public string UserId { get; set; }

    [ForeignKey("UserId")]
    public AppUser User { get; set; }

    public int ClinicId { get; set; }

    [ForeignKey("ClinicId")]
    public Clinic Clinic { get; set; }
    public decimal TotalAmount { get; set; }

    public string PaymentMethod { get; set; } = "COD";

    public string Status { get; set; } = "Pending";
    

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public ICollection<OrderItem> Items { get; set; }
}
