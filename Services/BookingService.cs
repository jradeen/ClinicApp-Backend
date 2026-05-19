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

        DateTime bookingTimeUtc = createBookingDto.AppointmentDateTime.ToUniversalTime();


        // Convert the incoming time to UTC before comparing
        if (bookingTimeUtc <= DateTime.UtcNow)
            throw new Exception("Invalid appointment time.");

        var isAvailable = await _bookingRepo.IsSlotAvailableAsync(
                createBookingDto.MedicalServiceId,
                bookingTimeUtc,
                medicalService.Duration + 10,//the 10 min is a buffer
                medicalService.AvailableStaffCapacity
            );

        if (!isAvailable)
            throw new Exception("This time slot is already booked for this service");

        var booking = new Booking
        {
            MedicalServiceId = createBookingDto.MedicalServiceId,
            AppointmentDateTime = bookingTimeUtc,
            UserId = userId
        };


        var result = await _bookingRepo.CreateAsync(booking);
        return ToBookingResponseDto(result);
    }

    public async Task<List<string>> GetAvailableSlotsAsync(int medicalServiceId, DateOnly date)
    {
        var medicalService = await _medicalServiceRepo.GetByIdAsync(medicalServiceId);

        if (medicalService == null)
            throw new KeyNotFoundException("Medical service not found");

        var clinic = medicalService.Clinic;
        if (clinic == null)
            throw new Exception("Clinic data not loaded for this service");


        //the fontend send just the date without time (e.g 2026-7-25 00:00:00) so we combine it with the clinic opening and closing time (08:30:00 -> 16:30:00)  
        var startDateTime = date.ToDateTime(TimeOnly.FromTimeSpan(clinic.OpeningTime));
        var endDateTime = date.ToDateTime(TimeOnly.FromTimeSpan(clinic.ClosingTime));

        var availableSlots = new List<string>();
        var currentSlot = startDateTime;
        int slotStepMinutes = medicalService.Duration + 10;

        while (currentSlot.AddMinutes(slotStepMinutes) <= endDateTime)
        {
            //if the user booked smth for today it doesnt generate slots in the past
            if (currentSlot.ToUniversalTime() > DateTime.UtcNow)
            {
                var isAvailable = await _bookingRepo.IsSlotAvailableAsync(
                        medicalServiceId,
                        currentSlot.ToUniversalTime(),
                        slotStepMinutes,
                        medicalService.AvailableStaffCapacity
                    );
                if (isAvailable)
                {
                    availableSlots.Add(currentSlot.ToString("HH:mm"));
                }
            }

            currentSlot = currentSlot.AddMinutes(slotStepMinutes);

        }

        return availableSlots;
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
    public async Task<bool> CancelBooking(int bookingId, string userId)
    {

        var booking = await _bookingRepo.GetByIdAsync(bookingId);
        if (booking == null) return false;

        if (userId != booking.UserId)
            throw new UnauthorizedAccessException("You don't have permission to cancel this booking.");

        booking.Status = "Cancelled";
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
