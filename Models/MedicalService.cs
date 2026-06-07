using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ClinicApp.API.Models;

public class MedicalService
{
    public int Id { get; set; }

    [Required]
    public string Name { get; set; }

    public string Description { get; set; }

    public decimal Price { get; set; }

    public int Duration { get; set; } // minutes

    public int ClinicId { get; set; }

    [ForeignKey("ClinicId")]
    public Clinic Clinic { get; set; }

    public ICollection<MedicalServiceTag> MedicalServiceTags { get; set; } = new List<MedicalServiceTag>();

    public string ImageUrl { get; set; } = "uploads/placeholders/default.jpg";

}
