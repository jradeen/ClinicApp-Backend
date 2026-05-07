using System;

namespace ClinicApp.API.DTOs.Booking;

public class BookingResponseDto
{
    public int Id { get; set; }
    public int MedicalServiceId { get; set; }
    public string MedicalServiceName { get; set; }
    public string ClinicName  { get; set; }
    public string? PatientEmail { get; set; }
    public string? PatientName { get; set; }
    public string? PatientPhone { get; set; }
    public DateTime AppointmentDateTime { get; set; }
    public string Status { get; set; }
    public string PaymentStatus { get; set; }
}
