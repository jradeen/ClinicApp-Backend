public class StaffResponseDto
{
    public int Id { get; set; }
    public string Name { get; set; }
    public int ClinicId { get; set; }
    public string ClinicName { get; set; }
    public List<StaffServiceDto> Services { get; set; } = new(); 
}