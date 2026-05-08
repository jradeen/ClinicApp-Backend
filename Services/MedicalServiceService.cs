using System;
using ClinicApp.API.DTOs.MedicalService;
using ClinicApp.API.Interfaces.IClinic;
using ClinicApp.API.Interfaces.IMedicalService;
using ClinicApp.API.Models;

namespace ClinicApp.API.Services;

public class MedicalServiceService : IMedicalServiceService
{
    private readonly IMedicalServiceRepository _medicalServiceRepo;
    private readonly IClinicRepository _clinicRepo;
    public MedicalServiceService(IMedicalServiceRepository medicalServiceRepo, IClinicRepository clinicRepo)
    {
        _clinicRepo = clinicRepo;
        _medicalServiceRepo = medicalServiceRepo;

    }
    public async Task<MedicalServiceResponseDto> CreateAsync(CreateMedicalServiceDto createMedicalServiceDto, string ownerId)
    {
        var clinic = await _clinicRepo.GetByOwnerIdAsync(ownerId);
        if (clinic == null) return null;

        var medicalService = new MedicalService
        {
            Name = createMedicalServiceDto.Name,
            Description = createMedicalServiceDto.Description,
            Price = createMedicalServiceDto.Price,
            Duration = createMedicalServiceDto.Duration,
            ClinicId = clinic.Id,
        };

        var result = await _medicalServiceRepo.CreateAsync(medicalService);
        return ToMedicalResponseDto(result);
    }

    public async Task<List<MedicalServiceResponseDto>> GetAllAsync()
    {
        var medicalServices= await _medicalServiceRepo.GetAllAsync();
        return medicalServices.Select(ToMedicalResponseDto).ToList();
    }

    public async Task<List<MedicalServiceResponseDto>> GetByClinicAsync(int clinicId)
    {
        var medicalServices=await _medicalServiceRepo.GetByClinicIdAsync(clinicId);
        return medicalServices.Select(ToMedicalResponseDto).ToList();
    }
    private MedicalServiceResponseDto ToMedicalResponseDto(MedicalService medicalService)
    {
        return new MedicalServiceResponseDto
        {
            Id = medicalService.Id,
            Name = medicalService.Name,
            Description = medicalService.Description,
            Price = medicalService.Price,
            Duration = medicalService.Duration,
            ClinicName = medicalService.Clinic?.Name ?? "Clinic Name unavailable",
            ClinicId = medicalService.ClinicId,
        };
    }
}
