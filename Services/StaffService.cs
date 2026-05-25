// Services/StaffService.cs
using ClinicApp.API.Interfaces.IClinic;
using ClinicApp.API.Interfaces.IMedicalService;
using ClinicApp.API.Models;

public class StaffService : IStaffService
{
    private readonly IStaffRepository _staffRepo;
    private readonly IClinicRepository _clinicRepo;
    private readonly IMedicalServiceRepository _medicalServiceRepo;

    public StaffService(IStaffRepository staffRepo, IClinicRepository clinicRepo, IMedicalServiceRepository medicalServiceRepo)
    {
        _staffRepo = staffRepo;
        _clinicRepo = clinicRepo;
        _medicalServiceRepo = medicalServiceRepo;
    }

    public async Task<List<StaffResponseDto>> GetByOwnerIdAsync(string ownerId)
    {
        var clinic = await _clinicRepo.GetByOwnerIdAsync(ownerId);
        if (clinic == null) throw new KeyNotFoundException("Clinic not found for this owner");

        var staff = await _staffRepo.GetByClinicIdAsync(clinic.Id);
        return staff.Select(ToResponseDto).ToList();
    }

    public async Task<StaffResponseDto> GetByIdAsync(int id, string ownerId)
    {
        var clinic = await _clinicRepo.GetByOwnerIdAsync(ownerId);
        if (clinic == null) throw new KeyNotFoundException("Clinic not found for this owner");//

        var staffMember = await _staffRepo.GetByIdAsync(id);
        if (staffMember == null) throw new KeyNotFoundException("Staff member not found");
        if (staffMember.ClinicId != clinic.Id) throw new UnauthorizedAccessException("You don't have permission to view this clinic staff");

        return ToResponseDto(staffMember);
    }

    public async Task<StaffResponseDto> CreateAsync(CreateStaffDto createDto, string ownerId)
    {
        var clinic = await _clinicRepo.GetByOwnerIdAsync(ownerId);
        if (clinic == null)
            throw new KeyNotFoundException("Clinic not found for this owner");

        var services = await _medicalServiceRepo.GetListByIdsAsync(createDto.ServiceIds);

        if (services.Count != createDto.ServiceIds.Count)
        {
            var notFoundIds = createDto.ServiceIds
                .Except(services.Select(s => s.Id))
                .ToList();
            throw new KeyNotFoundException($"Services not found: {string.Join(", ", notFoundIds)}");
        }

        var unauthorizedServices = services
            .Where(s => s.ClinicId != clinic.Id)
            .ToList();

        if (unauthorizedServices.Any())
        {
            var unauthorizedIds = unauthorizedServices.Select(s => s.Id).ToList();
            throw new UnauthorizedAccessException($"Services don't belong to your clinic: {string.Join(", ", unauthorizedIds)}");
        }

        var staff = new Staff
        {
            Name = createDto.Name,
            ClinicId = clinic.Id,
            StaffServices = createDto.ServiceIds.Select(serviceId => new StaffServices
            {
                MedicalServiceId = serviceId
            }).ToList()
        };

        var result = await _staffRepo.CreateAsync(staff);
        return ToResponseDto(result);
    }

    public async Task<StaffResponseDto> UpdateAsync(int id, UpdateStaffDto updateDto, string ownerId)
    {
        var clinic = await _clinicRepo.GetByOwnerIdAsync(ownerId);
        if (clinic == null) throw new KeyNotFoundException("Clinic not found for this owner");

        var staffMember = await _staffRepo.GetByIdAsync(id);
        if (staffMember == null) throw new KeyNotFoundException("Staff member not found");
        if (staffMember.ClinicId != clinic.Id) throw new UnauthorizedAccessException("You don't have permission to alter in clinic");

        var result = await _staffRepo.UpdateAsync(id, updateDto);
        if (result == null) throw new KeyNotFoundException("Staff member not found");
        return ToResponseDto(result);
    }

    public async Task<bool> DeleteAsync(int id, string ownerId)
    {
        var clinic = await _clinicRepo.GetByOwnerIdAsync(ownerId);
        if (clinic == null) throw new KeyNotFoundException("Clinic not found for this owner");


        var staffMember = await _staffRepo.GetByIdAsync(id);
        if (staffMember == null) throw new KeyNotFoundException("Staff member not found");
        if (staffMember.ClinicId != clinic.Id) throw new UnauthorizedAccessException("You don't have permission to delete staff from this clinic");

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
                .Select(ss => new StaffServiceDto
                {
                    Id = ss.MedicalServiceId,
                    Name = ss.MedicalService?.Name ?? "Unknown Service"
                })
                .ToList() ?? new()
        };
    }
}