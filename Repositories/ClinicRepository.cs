using System;
using ClinicApp.API.Data;
using ClinicApp.API.Interfaces.IClinic;
using ClinicApp.API.Models;
using Microsoft.EntityFrameworkCore;

namespace ClinicApp.API.Repositories;

public class ClinicRepository : IClinicRepository
{
    private readonly AppDbContext _context;
    public ClinicRepository(AppDbContext context)
    {
        _context = context;
    }
    public async Task<Clinic> CreateAsync(Clinic clinic)
    {
        await _context.Clinics.AddAsync(clinic);
        await _context.SaveChangesAsync();
        return clinic;
    }

    public async Task<List<Clinic>> GetAllAsync()
    {
        return await _context.Clinics
        .Include(c => c.ClinicTags)
        .ThenInclude(ct => ct.Tag)
        .ToListAsync();
    }

    public async Task<Clinic?> GetByIdAsync(int id)
    {
        return await _context.Clinics
        .Include(c => c.Owner)
        .Include(c => c.ClinicTags)
        .ThenInclude(ct => ct.Tag)
        .FirstOrDefaultAsync(c => c.Id == id);
    }

    public async Task<Clinic?> GetByOwnerIdAsync(string ownerId)
    {
        return await _context.Clinics
        .Include(c => c.ClinicTags)
        .ThenInclude(ct => ct.Tag)
        .FirstOrDefaultAsync(c => c.OwnerId == ownerId);
    }

    public async Task UpdateAsync(Clinic clinic)
    {
        _context.Clinics.Update(clinic);
        await _context.SaveChangesAsync();
    }
}
