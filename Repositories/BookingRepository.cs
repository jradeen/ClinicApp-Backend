using System;
using ClinicApp.API.Data;
using ClinicApp.API.Interfaces.IBooking;
using ClinicApp.API.Models;
using Microsoft.EntityFrameworkCore;

namespace ClinicApp.API.Repositories;

public class BookingRepository : IBookingRepository
{
    private readonly AppDbContext _context;
    public BookingRepository(AppDbContext context)
    {
        _context = context;
    }
    public async Task<Booking> CreateAsync(Booking booking)
    {
        await _context.Bookings.AddAsync(booking);
        await _context.SaveChangesAsync();
        return booking;
    }

    public async Task<List<Booking>> GetByClinicOwnerIdAsync(string ownerId)
    {
        return await _context.Bookings
                .Include(b => b.User)
                .Include(b => b.MedicalService)
                    .ThenInclude(s => s.Clinic)
                .Where(b => b.MedicalService.Clinic.OwnerId == ownerId)
                .AsNoTracking()
                .ToListAsync();
    }

    public async Task<Booking> GetByIdAsync(int bookingId)
    {
        var booking = await _context.Bookings.Include(b => b.MedicalService)
                        .ThenInclude(m => m.Clinic)
                        .FirstOrDefaultAsync(b => b.Id == bookingId);

        if (booking == null) return null;
        return booking;
    }

    public async Task<List<Booking>> GetByUserIdAsync(string userId)
    {
        return await _context.Bookings
        .Include(b => b.MedicalService)
        .ThenInclude(s => s.Clinic)
        .Where(b => b.UserId == userId)
        .AsNoTracking()
        .ToListAsync();

    }


    public async Task<bool> IsSlotAvailableAsync(int medicalServiceId, DateTime startingTime, int durationMinutes, int maxCapacity)
    {
        var endingTime = startingTime.AddMinutes(durationMinutes);

        var activeBookingsOverlaps = await _context.Bookings
            .CountAsync(b =>
                b.MedicalServiceId == medicalServiceId &&
                b.Status != "Cancelled" &&
                startingTime < b.AppointmentDateTime.AddMinutes(durationMinutes) &&
                endingTime > b.AppointmentDateTime);

        return activeBookingsOverlaps < maxCapacity;
    }

    public async Task UpdateAsync(Booking booking)
    {
        _context.Bookings.Update(booking);
        await _context.SaveChangesAsync();
    }
}
