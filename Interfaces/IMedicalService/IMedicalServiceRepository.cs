using System;
using ClinicApp.API.Models;

namespace ClinicApp.API.Interfaces.IMedicalService;

public interface IMedicalServiceRepository
{
    Task<MedicalService> CreateAsync(MedicalService service);
    Task<List<MedicalService>> GetByClinicIdAsync(int clinicId);
    Task<MedicalService?> GetByIdAsync(int MedicalServiceId);
    Task<MedicalService?> DeleteAsync(int id);
    Task UpdateAsync(MedicalService medicalService);
    Task<List<MedicalService>> GetAllAsync();
    Task<List<MedicalService>> GetListByIdsAsync(List<int> ids);


}
