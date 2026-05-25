namespace ClinicApp.API.Models;

public class Staff
{
    public int Id { get; set; }
    public string Name { get; set; }
    public int ClinicId { get; set; }
    public Clinic Clinic { get; set; }

    public ICollection<StaffServices> StaffServices { get; set; } // which services they handle
}