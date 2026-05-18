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
    private readonly IWebHostEnvironment _env;

    public MedicalServiceService(IWebHostEnvironment environment,IMedicalServiceRepository medicalServiceRepo, IClinicRepository clinicRepo)
    {
        _clinicRepo = clinicRepo;
        _medicalServiceRepo = medicalServiceRepo;
        _env = environment;

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
            ImageUrl = !string.IsNullOrEmpty(createMedicalServiceDto.ImageUrl) ? createMedicalServiceDto.ImageUrl : ""

        };

        var result = await _medicalServiceRepo.CreateAsync(medicalService);
        return ToMedicalResponseDto(result);
    }


    public async Task<List<MedicalServiceResponseDto>> GetAllAsync()
    {
        var medicalServices = await _medicalServiceRepo.GetAllAsync();
        return medicalServices.Select(ToMedicalResponseDto).ToList();
    }

    public async Task<List<MedicalServiceResponseDto>> GetByClinicAsync(int clinicId)
    {
        var medicalServices = await _medicalServiceRepo.GetByClinicIdAsync(clinicId);
        return medicalServices.Select(ToMedicalResponseDto).ToList();
    }
    public async Task<bool> DeleteMedicalServiceAsync(int medicalServiceId, string ownerId)
    {
        var medicalService = await _medicalServiceRepo.GetByIdAsync(medicalServiceId);
        if (medicalService == null)
            return false;
        if (medicalService.Clinic.OwnerId != ownerId)
            throw new UnauthorizedAccessException("You don't have permission to delete this medical service");

        await _medicalServiceRepo.DeleteAsync(medicalServiceId);
        return true;

    }

    public async Task<MedicalServiceResponseDto> UpdateMedicalServiceAsync(int medicalServiceId, UpdateMedicalServiceDto updateDto, string ownerId)
    {
        var medicalService = await _medicalServiceRepo.GetByIdAsync(medicalServiceId);
        if (medicalService == null)
            return null;
        if (medicalService.Clinic.OwnerId != ownerId)
            throw new UnauthorizedAccessException("You don't have permission to alter this medical service");

        medicalService.Name = updateDto.Name;
        medicalService.Description = updateDto.Description;
        medicalService.Price = updateDto.Price;
        medicalService.Duration = updateDto.Duration;
        if (!string.IsNullOrEmpty(updateDto.ImageUrl) && medicalService.ImageUrl != updateDto.ImageUrl)
        {
            var oldFilePath = Path.Combine(_env.WebRootPath, medicalService.ImageUrl);

            // Check if the old file physically exists and delete it
            if (System.IO.File.Exists(oldFilePath))
            {
                System.IO.File.Delete(oldFilePath);
            }
            medicalService.ImageUrl = updateDto.ImageUrl;
        }
        await _medicalServiceRepo.UpdateAsync(medicalService);
        return ToMedicalResponseDto(medicalService);

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
            AvailableStaffCapacity = medicalService.AvailableStaffCapacity,
            ImageUrl = medicalService.ImageUrl,
        };
    }
}
