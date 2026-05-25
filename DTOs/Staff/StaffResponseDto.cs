public class StaffResponseDto
{
    public int Id { get; set; }
    public string Name { get; set; }
    public int ClinicId { get; set; }
    public string ClinicName { get; set; }
    public List<string> Services { get; set; } = new(); 
}