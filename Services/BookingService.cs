using System;
using ClinicApp.API.DTOs.Booking;
using ClinicApp.API.Interfaces.IBooking;
using ClinicApp.API.Interfaces.IMedicalService;
using ClinicApp.API.Models;

namespace ClinicApp.API.Services;

public class BookingService : IBookingService
{
    private readonly IBookingRepository _bookingRepo;
    private readonly IMedicalServiceRepository _medicalServiceRepo;

    public BookingService(IMedicalServiceRepository medicalServiceRepo, IBookingRepository bookingRepo)
    {
        _bookingRepo = bookingRepo;
        _medicalServiceRepo = medicalServiceRepo;
    }
    public async Task<BookingResponseDto> CreateAsync(CreateBookingDto createBookingDto, string userId)
    {
        var medicalService = await _medicalServiceRepo.GetByIdAsync(createBookingDto.MedicalServiceId);

        if (medicalService == null)
            throw new Exception("Medical service not found");

        // Convert the incoming time to UTC before comparing
        if (createBookingDto.AppointmentDateTime.ToUniversalTime() <= DateTime.UtcNow)
            throw new Exception("Invalid appointment time.");

        var isAvailable = await _bookingRepo.IsSlotAvailableAsync(createBookingDto.MedicalServiceId, createBookingDto.AppointmentDateTime);
        if (!isAvailable)
            throw new Exception("This time slot is already booked for this service");

        var booking = new Booking
        {
            MedicalServiceId = createBookingDto.MedicalServiceId,
            AppointmentDateTime = createBookingDto.AppointmentDateTime,
            UserId = userId
        };


        var result = await _bookingRepo.CreateAsync(booking);
        return ToBookingResponseDto(result);
    }

    public async Task<List<BookingResponseDto>> GetClinicBookingsAsync(string ownerId)
    {
        var bookings = await _bookingRepo.GetByClinicOwnerIdAsync(ownerId);

        return bookings.Select(ToBookingResponseDto).ToList();
    }

    public async Task<List<BookingResponseDto>> GetUserBookingsAsync(string userId)
    {
        var bookings = await _bookingRepo.GetByUserIdAsync(userId);

        return bookings.Select(ToBookingResponseDto).ToList();
    }

    public async Task<bool> UpdateStatusAsync(int bookingId, string newStatus, string ownerId)
    {
        var validStatuses = new[] { "Confirmed", "Cancelled", "Completed" };
        if (!validStatuses.Contains(newStatus)) throw new Exception("Invalid status name.{ Confirmed, Cancelled, Completed }");

        var booking = await _bookingRepo.GetByIdAsync(bookingId);
        if (booking == null) return false;

        if (ownerId != booking.MedicalService.Clinic.OwnerId)
            throw new UnauthorizedAccessException("You don't have permission to change this booking.");

        booking.Status = newStatus;
        await _bookingRepo.UpdateAsync(booking);
        return true;

    }

    private BookingResponseDto ToBookingResponseDto(Booking booking)
    {
        return new BookingResponseDto
        {
            Id = booking.Id,
            MedicalServiceId = booking.MedicalServiceId,
            MedicalServiceName = booking.MedicalService?.Name ?? "Service Name Unavailable",
            ClinicName = booking.MedicalService?.Clinic?.Name ?? "Clinic Name Unavailable",
            PatientEmail = booking.User?.Email ?? "No Email",
            PatientName = booking.User?.UserName ?? "Deleted User",
            PatientPhone = booking.User?.PhoneNumber ?? "No Phone Number",
            AppointmentDateTime = booking.AppointmentDateTime,
            PaymentStatus = booking.PaymentStatus,
            Status = booking.Status
        };
    }
}
