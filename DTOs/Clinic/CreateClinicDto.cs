using System;
using System.ComponentModel.DataAnnotations;

namespace ClinicApp.API.DTOs.Clinic;

public class CreateClinicDto
{
    [Required, StringLength(100, MinimumLength = 2)]
    public string Name { get; set; }

    [Required, StringLength(500)]
    public string Description { get; set; }

    [Required, StringLength(200)]
    public string Location { get; set; }
    public TimeSpan OpeningTime { get; set; }
    public TimeSpan ClosingTime { get; set; }

    [Required]
    [Phone]
    public string PhoneNumber { get; set; } = string.Empty;
    
    [Required]
    public List<int> TagIds { get; set; } = new();

    public string? ImageUrl { get; set; }
}
