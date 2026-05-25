public interface IStaffService
{
    Task<List<StaffResponseDto>> GetByOwnerIdAsync(string ownerId);
    Task<StaffResponseDto> GetByIdAsync(int id,string ownerId);
    Task<StaffResponseDto> CreateAsync(CreateStaffDto dto, string ownerId);
    Task<StaffResponseDto> UpdateAsync(int id, UpdateStaffDto dto ,string ownerId);
    Task<bool> DeleteAsync(int id, string ownerId);
}