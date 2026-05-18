using System;
using ClinicApp.API.Models;

namespace ClinicApp.API.Interfaces.IBooking;

public interface IBookingRepository
{
    Task<Booking> CreateAsync(Booking booking);
    Task<Booking> GetByIdAsync(int bookingId);
    Task<List<Booking>> GetByUserIdAsync(string userId);
    Task<List<Booking>> GetByClinicOwnerIdAsync(string ownerId);
    Task<bool> IsSlotAvailableAsync(int medicalServiceId, DateTime startingTime, int durationMinutes, int maxCapacity);
    Task UpdateAsync(Booking booking);

}
