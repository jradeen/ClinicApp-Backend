using System;
using System.ComponentModel.DataAnnotations;

namespace ClinicApp.API.DTOs.Clinic;

public class ClinicResponseDto
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public string Location { get; set; }

    [Phone]
    public string PhoneNumber { get; set; } = string.Empty;

    public string OwnerId { get; set; }
    public string ImageUrl { get; set; } = string.Empty;

}
