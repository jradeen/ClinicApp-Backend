using System;
using ClinicApp.API.DTOs.Clinic;

namespace ClinicApp.API.Interfaces.IClinic;

public interface IClinicService
{
    Task<ClinicResponseDto> CreateClinicAsync(CreateClinicDto createClinicDto, string ownerId);
    Task<List<ClinicResponseDto>> GetAllAsync();
    Task<ClinicResponseDto>UpdateClinicAsync(int clinicId,UpdateClinicDto updateDto, string ownerId);
    Task<ClinicResponseDto?> GetByIdAsync(int id);
    Task<ClinicResponseDto?> GetByOwnerIdAsync(string ownerId);
}
