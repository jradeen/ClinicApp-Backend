using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ClinicApp.API.Models
{
    public class Booking
    {
        public int Id { get; set; }

        public string UserId { get; set; }
        
        [ForeignKey("UserId")]
        public AppUser User { get; set; }

        public int MedicalServiceId { get; set; }

        [ForeignKey("MedicalServiceId")]
        public MedicalService MedicalService { get; set; }

        public DateTime AppointmentDateTime { get; set; }

        public string Status { get; set; } = "Pending"; // Pending / Confirmed / Cancelled / Completed

        public string PaymentMethod { get; set; } = "COD";

        public string PaymentStatus { get; set; } = "Pending";// Pending / Paid
    }
}