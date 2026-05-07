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

        public string OwnerId { get; set; }

        [ForeignKey("OwnerId")]
        public AppUser Owner { get; set; }
        public ICollection<Product> Products { get; set; }
        public ICollection<MedicalService> Services { get; set; }
    }
}