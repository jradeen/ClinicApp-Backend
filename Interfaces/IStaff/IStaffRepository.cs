using ClinicApp.API.Models;

public interface IStaffRepository
{
    Task<List<Staff>> GetByClinicIdAsync(int clinicId);
    Task<Staff?> GetByIdAsync(int id);
    Task<Staff> CreateAsync(Staff staff);
    Task<Staff?> UpdateAsync(int id, UpdateStaffDto dto);
    Task<bool> DeleteAsync(int id);
    Task<int?> GetAvailableStaffAsync(int medicalServiceId, DateTime startingTime, int durationMinutes);
}