public class CreateStaffDto
{
    public string Name { get; set; }
    public List<int> ServiceIds { get; set; } = new(); // which services they handle
}