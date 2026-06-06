using System;
using System.Diagnostics;
using ClinicApp.API.DTOs.Booking;
using ClinicApp.API.Interfaces.IBooking;
using ClinicApp.API.Interfaces.IClinic;
using ClinicApp.API.Interfaces.IMedicalService;
using ClinicApp.API.Models;

namespace ClinicApp.API.Services;

public class BookingService : IBookingService
{
    private readonly IBookingRepository _bookingRepo;
    private readonly IStaffRepository _staffRepo;
    private readonly IMedicalServiceRepository _medicalServiceRepo;
    private readonly IClinicRepository _clinicRepo;
    private readonly IEmailService _emailService;
    private readonly ILogger<BookingService> _logger;

    public BookingService(IClinicRepository clinicRepo, ILogger<BookingService> logger, IEmailService emailService, IStaffRepository staffRepo, IMedicalServiceRepository medicalServiceRepo, IBookingRepository bookingRepo)
    {
        _bookingRepo = bookingRepo;
        _staffRepo = staffRepo;
        _medicalServiceRepo = medicalServiceRepo;
        _clinicRepo = clinicRepo;
        _emailService = emailService;
        _logger = logger;
    }


    public async Task<BookingResponseDto> CreateAsync(CreateBookingDto createBookingDto, string userId)
    {
        var medicalService = await _medicalServiceRepo.GetByIdAsync(createBookingDto.MedicalServiceId);

        if (medicalService == null)
            throw new Exception("Medical service not found");

        DateTime bookingTimeUtc = createBookingDto.AppointmentDateTime.ToUniversalTime();

        if (bookingTimeUtc <= DateTime.UtcNow)
            throw new Exception("Invalid appointment time.");

        var availableStaffId = await _staffRepo.GetAvailableStaffAsync(
                createBookingDto.MedicalServiceId,
                bookingTimeUtc,
                medicalService.Duration + 10
            );

        if (availableStaffId == null)
            throw new Exception("No available staff for this time slot");

        var booking = new Booking
        {
            MedicalServiceId = createBookingDto.MedicalServiceId,
            AppointmentDateTime = bookingTimeUtc,
            UserId = userId,
            StaffId = availableStaffId,
            MedicalServiceDuration = medicalService.Duration + 10
        };

        var result = await _bookingRepo.CreateAsync(booking);

        var clinic = await _clinicRepo.GetByIdAsync(medicalService.ClinicId);

        var localTime = TimeZoneInfo.ConvertTimeFromUtc(
            booking.AppointmentDateTime,
            TimeZoneInfo.FindSystemTimeZoneById("Jordan Standard Time")
        );

        _logger.LogWarning("Owner email: {Email}", clinic?.Owner?.Email);
        if (clinic?.Owner?.Email != null)
        {
            await _emailService.SendAsync(
                clinic.Owner.Email,
                clinic.Owner.UserName ?? "Clinic Owner",
                "New booking made",
                $"Hi {clinic.Owner.UserName ?? "Clinic Owner"}, {booking.User?.UserName ?? "A patient"} has booked {medicalService.Name} on {localTime:dd/MM/yyyy HH:mm}."
            );
        }
        else
        {
            _logger.LogWarning("Email was null, skipping notification ");
        }

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
                var availableStaffId = await _staffRepo.GetAvailableStaffAsync(
                    medicalServiceId,
                    currentSlot.ToUniversalTime(),
                    slotStepMinutes
                );
                if (availableStaffId != null)
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
        var validStatuses = new[] { "Confirmed", "Cancelled", "Completed", "Pending" };
        if (!validStatuses.Contains(newStatus)) throw new Exception("Invalid status name.{ Confirmed, Cancelled, Completed }");

        var booking = await _bookingRepo.GetByIdAsync(bookingId);
        if (booking == null) return false;

        if (ownerId != booking.MedicalService.Clinic.OwnerId)
            throw new UnauthorizedAccessException("You don't have permission to change this booking.");

        booking.Status = newStatus;
        await _bookingRepo.UpdateAsync(booking);
        _logger.LogWarning("userEmail: {Email}", booking.User?.Email);
        var localTime = TimeZoneInfo.ConvertTimeFromUtc(
            booking.AppointmentDateTime,
            TimeZoneInfo.FindSystemTimeZoneById("Jordan Standard Time")
        );
        if (booking.User?.Email != null)
        {
            await _emailService.SendAsync(
                booking.User.Email,
                booking.User.UserName ?? "User",
                "Your booking status has been updated",
                $"Hi {booking.User.UserName ?? "User"}, your booking for {booking.MedicalService?.Name}on {localTime:dd/MM/yyyy HH:mm} is now {newStatus}."
            );
        }
        else
        {
            _logger.LogWarning("Email was null, skipping notification ");
        }

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

        var localTime = TimeZoneInfo.ConvertTimeFromUtc(
            booking.AppointmentDateTime,
            TimeZoneInfo.FindSystemTimeZoneById("Jordan Standard Time")
        );

        if (booking.MedicalService?.Clinic?.Owner?.Email != null)
        {
            await _emailService.SendAsync(
                booking.MedicalService.Clinic.Owner.Email,
                booking.MedicalService.Clinic.Owner.UserName ?? "Service Name Unavailable",
                "A booking has been cancelled",
                $"Patient {booking.User?.UserName ?? "Unknown"} cancelled their {booking.MedicalService.Name} appointment on {localTime:dd/MM/yyyy HH:mm}."
            );
        }
        else
        {
            _logger.LogWarning("Email was null, skipping notification ");
        }

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
