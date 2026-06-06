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
        var booking = await _context.Bookings
                        .Include(b => b.User)
                        .Include(b => b.MedicalService)
                        .ThenInclude(m => m.Clinic)
                        .ThenInclude(c => c.Owner)
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


    public async Task UpdateAsync(Booking booking)
    {
        _context.Bookings.Update(booking);
        await _context.SaveChangesAsync();
    }
}
