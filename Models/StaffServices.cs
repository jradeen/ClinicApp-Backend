namespace ClinicApp.API.Models;

public class StaffServices
{
    public int StaffId { get; set; }
    public Staff Staff { get; set; }

    public int MedicalServiceId { get; set; }
    public MedicalService MedicalService { get; set; }
}