using System;

namespace ClinicApp.API.DTOs.MedicalService;

public class MedicalServiceResponseDto
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public decimal Price { get; set; }
    public int Duration { get; set; }
    public string ClinicName { get; set; }
    public int ClinicId { get; set; }
    public string ImageUrl { get; set; } = string.Empty;

}
