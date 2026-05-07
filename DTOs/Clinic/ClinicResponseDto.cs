using System;

namespace ClinicApp.API.DTOs.Clinic;

public class ClinicResponseDto
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public string Location { get; set; }
    public string OwnerId { get; set; }
}
