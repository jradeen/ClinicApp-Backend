using System;
using ClinicApp.API.DTOs.MedicalService;

namespace ClinicApp.API.Interfaces.IMedicalService;

public interface IMedicalServiceService
{
    Task<MedicalServiceResponseDto> CreateAsync(CreateMedicalServiceDto createMedicalServiceDto, string ownerId);
    Task<List<MedicalServiceResponseDto>> GetByClinicAsync(int clinicId);
    Task<bool> DeleteMedicalServiceAsync(int medicalServiceid,string ownerId);
    Task<MedicalServiceResponseDto> UpdateMedicalServiceAsync(int medicalServiceId, UpdateMedicalServiceDto updateDto, string ownerId);
    Task<List<MedicalServiceResponseDto>> GetAllAsync();

}
