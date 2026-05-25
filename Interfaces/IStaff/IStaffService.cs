public interface IStaffService
{
    Task<List<StaffResponseDto>> GetByClinicIdAsync(int clinicId);
    Task<StaffResponseDto> GetByIdAsync(int id);
    Task<StaffResponseDto> CreateAsync(CreateStaffDto dto);
    Task<StaffResponseDto> UpdateAsync(int id, UpdateStaffDto dto);
    Task<bool> DeleteAsync(int id);
}