using System;
using ClinicApp.API.DTOs.Clinic;
using ClinicApp.API.Interfaces.IClinic;
using ClinicApp.API.Models;

namespace ClinicApp.API.Services;

public class ClinicService : IClinicService
{
    private readonly IClinicRepository _clinicRepo;
    public ClinicService(IClinicRepository clinicRepo)
    {
        _clinicRepo = clinicRepo;
    }
    public async Task<ClinicResponseDto> CreateClinicAsync(CreateClinicDto createClinicDto, string ownerId)
    {
        var clinicExists = await _clinicRepo.GetByOwnerIdAsync(ownerId);

        if (clinicExists != null)
        {
            return null;
        }
        var clinic = new Clinic
        {
            Name = createClinicDto.Name,
            Description = createClinicDto.Description,
            Location = createClinicDto.Location,
            OwnerId = ownerId
        };

        var result = await _clinicRepo.CreateAsync(clinic);
        return ToClinicResponseDto(result);
    }

    public async Task<List<ClinicResponseDto>> GetAllAsync()
    {
        var clinics = await _clinicRepo.GetAllAsync();
        return clinics.Select(ToClinicResponseDto).ToList();

    }

    public async Task<ClinicResponseDto?> GetByIdAsync(int id)
    {
        var clinic = await _clinicRepo.GetByIdAsync(id);
        if (clinic == null) return null;

        return ToClinicResponseDto(clinic);
    }

    private ClinicResponseDto ToClinicResponseDto(Clinic clinic)
    {
        return new ClinicResponseDto
        {
            Id = clinic.Id,
            Name = clinic.Name,
            Description = clinic.Description,
            Location = clinic.Location,
            OwnerId = clinic.OwnerId
        };
    }
}
