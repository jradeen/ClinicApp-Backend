using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ClinicApp.API.Models
{
    public class Clinic
    {
        public int Id { get; set; }

        [Required]
        public string Name { get; set; } = string.Empty;

        public string Description { get; set; } = string.Empty;

        public string Location { get; set; } = string.Empty;

        [Phone]
        public string PhoneNumber { get; set; } = string.Empty;

        [Required]
        public TimeSpan OpeningTime { get; set; } = new TimeSpan(9, 0, 0);

        [Required]
        public TimeSpan ClosingTime { get; set; } = new TimeSpan(17, 0, 0);
        
        public string OwnerId { get; set; }

        [ForeignKey("OwnerId")]
        public AppUser Owner { get; set; }
        public ICollection<Product> Products { get; set; }
        public ICollection<MedicalService> Services { get; set; }
        public ICollection<ClinicTag> ClinicTags { get; set; } = new List<ClinicTag>();

        public string ImageUrl { get; set; } = "uploads/placeholders/default.jpg";
    }
}