// Repositories/StaffRepository.cs
using ClinicApp.API.Data;
using ClinicApp.API.Models;
using Microsoft.EntityFrameworkCore;
// Repositories/StaffRepository.cs
public class StaffRepository : IStaffRepository
{
    private readonly AppDbContext _context;

    public StaffRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<List<Staff>> GetByClinicIdAsync(int clinicId)
    {
        return await _context.Staff
            .Include(s => s.StaffServices)
                .ThenInclude(ss => ss.MedicalService)
            .Where(s => s.ClinicId == clinicId)
            .ToListAsync();
    }

    public async Task<Staff?> GetByIdAsync(int id)
    {
        return await _context.Staff
            .Include(s => s.StaffServices)
                .ThenInclude(ss => ss.MedicalService)
            .Include(s => s.Clinic)
            .FirstOrDefaultAsync(s => s.Id == id);
    }

    public async Task<Staff> CreateAsync(Staff staff)
    {
        await _context.Staff.AddAsync(staff);
        await _context.SaveChangesAsync();
        return staff;
    }

    public async Task<Staff?> UpdateAsync(int id, UpdateStaffDto dto)
    {
        var staff = await _context.Staff
            .Include(s => s.StaffServices)
            .FirstOrDefaultAsync(s => s.Id == id);

        if (staff == null) return null;

        staff.Name = dto.Name;

        // remove old service assignments and replace with new ones
        _context.StaffServices.RemoveRange(staff.StaffServices);
        staff.StaffServices = dto.ServiceIds.Select(serviceId => new StaffServices
        {
            StaffId = id,
            MedicalServiceId = serviceId
        }).ToList();

        await _context.SaveChangesAsync();
        return staff;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var staff = await _context.Staff.FindAsync(id);
        if (staff == null) return false;

        _context.Staff.Remove(staff);
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<int?> GetAvailableStaffAsync(int medicalServiceId, DateTime startingTime, int durationMinutes)
    {
        var endingTime = startingTime.AddMinutes(durationMinutes);

        var eligibleStaffIds = await _context.StaffServices
            .Where(ss => ss.MedicalServiceId == medicalServiceId)
            .Select(ss => ss.StaffId)
            .ToListAsync();

        var availableStaffId = await _context.Staff
            .Where(s => eligibleStaffIds.Contains(s.Id))
            .Where(s => !_context.Bookings.Any(b =>
                b.StaffId == s.Id &&
                b.Status != "Cancelled" &&
                startingTime < b.AppointmentDateTime.AddMinutes(durationMinutes) &&
                endingTime > b.AppointmentDateTime))
            .Select(s => s.Id)
            .FirstOrDefaultAsync();

        return availableStaffId == 0 ? null : availableStaffId;
    }
}