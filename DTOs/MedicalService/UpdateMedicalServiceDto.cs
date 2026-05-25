using System;
using System.ComponentModel.DataAnnotations;

namespace ClinicApp.API.DTOs.MedicalService;

public class UpdateMedicalServiceDto
{
    [Required, StringLength(100)]
    public string Name { get; set; }

    [Required, MaxLength(500)]
    public string Description { get; set; }

    [Range(0.1, 5000)]
    public decimal Price { get; set; }

    [Range(5, 480)] // 5 mins and 8 hours
    public int Duration { get; set; }
    public string? ImageUrl { get; set; }

}
