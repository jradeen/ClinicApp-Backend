using System;
using ClinicApp.API.DTOs.Booking;

namespace ClinicApp.API.Interfaces.IBooking;

public interface IBookingService
{
    Task<BookingResponseDto> CreateAsync(CreateBookingDto createBookingDto, string userId);
    Task<List<BookingResponseDto>> GetUserBookingsAsync(string userId);
    Task<List<BookingResponseDto>> GetClinicBookingsAsync(string ownerId);
    Task<bool> UpdateStatusAsync(int bookingId, string newStatus, string ownerId);
    Task<List<string>> GetAvailableSlotsAsync(int medicalServiceId, DateTime date);

}
