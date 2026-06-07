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

    public TimeSpan OpeningTime { get; set; }
    public TimeSpan ClosingTime { get; set; }

    public string OwnerId { get; set; }
    public List<string> Tags { get; set; } = new();

    public string ImageUrl { get; set; } = string.Empty;

}
