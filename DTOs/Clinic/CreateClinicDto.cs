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
}
