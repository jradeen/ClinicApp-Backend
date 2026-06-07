using ClinicApp.API.Models;

public class MedicalServiceTag
{
    public int MedicalServiceId { get; set; }
    public MedicalService MedicalService { get; set; }
    public int TagId { get; set; }
    public Tag Tag { get; set; }
}