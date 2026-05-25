// Services/StaffService.cs
using ClinicApp.API.Models;

public class StaffService : IStaffService
{
    private readonly IStaffRepository _staffRepo;

    public StaffService(IStaffRepository staffRepo)
    {
        _staffRepo = staffRepo;
    }

    public async Task<List<StaffResponseDto>> GetByClinicIdAsync(int clinicId)
    {
        var staff = await _staffRepo.GetByClinicIdAsync(clinicId);
        return staff.Select(ToResponseDto).ToList();
    }

    public async Task<StaffResponseDto> GetByIdAsync(int id)
    {
        var staff = await _staffRepo.GetByIdAsync(id);
        if (staff == null) throw new KeyNotFoundException("Staff member not found");
        return ToResponseDto(staff);
    }

    public async Task<StaffResponseDto> CreateAsync(CreateStaffDto dto)
    {
        var staff = new Staff
        {
            Name = dto.Name,
            ClinicId = dto.ClinicId,
            StaffServices = dto.ServiceIds.Select(serviceId => new StaffServices
            {
                MedicalServiceId = serviceId
            }).ToList()
        };

        var result = await _staffRepo.CreateAsync(staff);
        return ToResponseDto(result);
    }

    public async Task<StaffResponseDto> UpdateAsync(int id, UpdateStaffDto dto)
    {
        var result = await _staffRepo.UpdateAsync(id, dto);
        if (result == null) throw new KeyNotFoundException("Staff member not found");
        return ToResponseDto(result);
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var deleted = await _staffRepo.DeleteAsync(id);
        if (!deleted) throw new KeyNotFoundException("Staff member not found");
        return true;
    }

    private StaffResponseDto ToResponseDto(Staff staff)
    {
        return new StaffResponseDto
        {
            Id = staff.Id,
            Name = staff.Name,
            ClinicId = staff.ClinicId,
            ClinicName = staff.Clinic?.Name ?? "Unknown Clinic",
            Services = staff.StaffServices?
                .Select(ss => ss.MedicalService?.Name ?? "Unknown Service")
                .ToList() ?? new()
        };
    }
}