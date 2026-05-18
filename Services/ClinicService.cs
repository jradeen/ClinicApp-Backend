using System;
using ClinicApp.API.DTOs.Clinic;
using ClinicApp.API.Interfaces.IClinic;
using ClinicApp.API.Models;

namespace ClinicApp.API.Services;

public class ClinicService : IClinicService
{
    private readonly IClinicRepository _clinicRepo;
    private readonly IWebHostEnvironment _env;

    public ClinicService(IClinicRepository clinicRepo,IWebHostEnvironment environment)
    {
        _clinicRepo = clinicRepo;
        _env=environment;
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
            PhoneNumber = createClinicDto.PhoneNumber,
            OwnerId = ownerId,
            ImageUrl = !string.IsNullOrEmpty(createClinicDto.ImageUrl) ? createClinicDto.ImageUrl : ""

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

    public async Task<ClinicResponseDto> UpdateClinicAsync(int clinicId, UpdateClinicDto updateDto, string ownerId)
    {
        var clinic = await _clinicRepo.GetByIdAsync(clinicId);
        if (clinic == null)
            return null;
        if (clinic.OwnerId != ownerId)
            throw new UnauthorizedAccessException("You don't have permission to alter this clinic");

        clinic.Name = updateDto.Name;
        clinic.Description = updateDto.Description;
        clinic.PhoneNumber = updateDto.PhoneNumber;
        clinic.Location = updateDto.Location;
        if (!string.IsNullOrEmpty(updateDto.ImageUrl) && clinic.ImageUrl != updateDto.ImageUrl)
        {
            var oldFilePath = Path.Combine(_env.WebRootPath, clinic.ImageUrl);

            if (System.IO.File.Exists(oldFilePath))
            {
                System.IO.File.Delete(oldFilePath);
            }
            clinic.ImageUrl = updateDto.ImageUrl;
        }

        await _clinicRepo.UpdateAsync(clinic);
        return ToClinicResponseDto(clinic);
    }

    private ClinicResponseDto ToClinicResponseDto(Clinic clinic)
    {
        return new ClinicResponseDto
        {
            Id = clinic.Id,
            Name = clinic.Name,
            Description = clinic.Description,
            OpeningTime = clinic.OpeningTime,
            ClosingTime = clinic.ClosingTime,
            PhoneNumber = clinic.PhoneNumber ?? "unavailable",
            Location = clinic.Location,
            OwnerId = clinic.OwnerId,
            ImageUrl = clinic.ImageUrl,
        };
    }
}
