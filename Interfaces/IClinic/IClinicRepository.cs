using System;
using ClinicApp.API.Models;

namespace ClinicApp.API.Interfaces.IClinic;

public interface IClinicRepository
{
    Task<Clinic> CreateAsync(Clinic clinic);
    Task<List<Clinic>> GetAllAsync();
    Task<Clinic?> GetByIdAsync(int id);
    Task<Clinic?> GetByOwnerIdAsync(string ownerId);
}
