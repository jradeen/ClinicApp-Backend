using ClinicApp.API.Models;

public class ClinicTag
{
    public int ClinicId { get; set; }
    public Clinic Clinic { get; set; }
    public int TagId { get; set; }
    public Tag Tag { get; set; }
}