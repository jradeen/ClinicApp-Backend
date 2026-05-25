public class UpdateStaffDto
{
    public string Name { get; set; }
    public List<int> ServiceIds { get; set; } = new();
}
