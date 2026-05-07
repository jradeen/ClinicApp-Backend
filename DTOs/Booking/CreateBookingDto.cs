using System;
using System.ComponentModel.DataAnnotations;

namespace ClinicApp.API.DTOs.Booking;

public class CreateBookingDto
{
    [Required]
    public int MedicalServiceId { get; set; }
    
    [Required]
    public DateTime AppointmentDateTime { get; set; }

}
