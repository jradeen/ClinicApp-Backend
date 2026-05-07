using System;
using ClinicApp.API.Data;
using ClinicApp.API.Interfaces.IMedicalService;
using ClinicApp.API.Models;
using Microsoft.EntityFrameworkCore;

namespace ClinicApp.API.Repositories;

public class MedicalServiceRepository : IMedicalServiceRepository
{
    private readonly AppDbContext _context;
    public MedicalServiceRepository(AppDbContext context)
    {
        _context = context;
    }
    public async Task<MedicalService> CreateAsync(MedicalService service)
    {
        await _context.MedicalServices.AddAsync(service);
        await _context.SaveChangesAsync();
        return service;
    }

    public async Task<List<MedicalService>> GetByClinicIdAsync(int clinicId)
    {
        return await _context.MedicalServices.Where(c => c.ClinicId == clinicId).ToListAsync();
    }

    public async Task<MedicalService?> GetByIdAsync(int MedicalServiceId)
    {
        return await _context.MedicalServices.FirstOrDefaultAsync(m => m.Id == MedicalServiceId);
    }
}
