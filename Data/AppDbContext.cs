using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using ClinicApp.API.Models;

namespace ClinicApp.API.Data
{
    public class AppDbContext : IdentityDbContext<AppUser>
    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options)
        {

        }

        public DbSet<Clinic> Clinics { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<MedicalService> MedicalServices { get; set; }
        public DbSet<Booking> Bookings { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderItem> OrderItems { get; set; }
        public DbSet<Staff> Staff { get; set; }
        public DbSet<StaffServices> StaffServices { get; set; }
        public DbSet<Tag> Tags { get; set; }
        public DbSet<ClinicTag> ClinicTags { get; set; }
        public DbSet<MedicalServiceTag> MedicalServiceTags { get; set; }
        public DbSet<ProductTag> ProductTags { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<StaffServices>().HasKey(ss => new { ss.StaffId, ss.MedicalServiceId });

            builder.Entity<ClinicTag>().HasKey(ct => new { ct.ClinicId, ct.TagId });

            builder.Entity<MedicalServiceTag>().HasKey(mt => new { mt.MedicalServiceId, mt.TagId });

            builder.Entity<ProductTag>().HasKey(pt => new { pt.ProductId, pt.TagId });


            builder.Entity<Tag>().HasData(
        // === Clinic Categories ===
        new Tag { Id = 1, Name = "Pediatrics", Category = "Clinic" },
        new Tag { Id = 2, Name = "Dentistry", Category = "Clinic" },
        new Tag { Id = 3, Name = "Cardiology", Category = "Clinic" },
        new Tag { Id = 4, Name = "Dermatology", Category = "Clinic" },
        new Tag { Id = 12, Name = "General Practice", Category = "Clinic" },
        new Tag { Id = 13, Name = "Orthopedics", Category = "Clinic" },
        new Tag { Id = 14, Name = "Gynecology & OB", Category = "Clinic" },
        new Tag { Id = 15, Name = "Psychiatry", Category = "Clinic" },
        new Tag { Id = 16, Name = "Ophthalmology", Category = "Clinic" },
        new Tag { Id = 17, Name = "Neurology", Category = "Clinic" },
        new Tag { Id = 18, Name = "Physiotherapy", Category = "Clinic" },
        new Tag { Id = 19, Name = "Endocrinology", Category = "Clinic" },
        new Tag { Id = 20, Name = "Nutrition & Dietetics", Category = "Clinic" },
        new Tag { Id = 21, Name = "Podiatry", Category = "Clinic" },
        new Tag { Id = 22, Name = "Aesthetics & Cosmetic", Category = "Clinic" },

        // === MedicalService Categories ===
        new Tag { Id = 5, Name = "Laboratory", Category = "MedicalService" },
        new Tag { Id = 6, Name = "Imaging", Category = "MedicalService" },
        new Tag { Id = 7, Name = "Urgent", Category = "MedicalService" },
        new Tag { Id = 8, Name = "Consultation", Category = "MedicalService" },
        new Tag { Id = 23, Name = "Blood Test", Category = "MedicalService" },
        new Tag { Id = 24, Name = "X-Ray", Category = "MedicalService" },
        new Tag { Id = 25, Name = "Ultrasound", Category = "MedicalService" },
        new Tag { Id = 26, Name = "Vaccination", Category = "MedicalService" },
        new Tag { Id = 27, Name = "Allergy Testing", Category = "MedicalService" },
        new Tag { Id = 28, Name = "MRI Scan", Category = "MedicalService" },
        new Tag { Id = 29, Name = "Teeth Cleaning", Category = "MedicalService" },
        new Tag { Id = 30, Name = "Cardiac Screening", Category = "MedicalService" },
        new Tag { Id = 31, Name = "Minor Surgery", Category = "MedicalService" },
        new Tag { Id = 32, Name = "Physical Therapy Session", Category = "MedicalService" },
        new Tag { Id = 33, Name = "Telehealth Consultation", Category = "MedicalService" },
        new Tag { Id = 34, Name = "Mental Health Therapy", Category = "MedicalService" },
        new Tag { Id = 35, Name = "Laser Treatment", Category = "MedicalService" },

        // === Product Categories ===
        new Tag { Id = 9, Name = "Supplement", Category = "Product" },
        new Tag { Id = 10, Name = "Equipment", Category = "Product" },
        new Tag { Id = 11, Name = "Skincare", Category = "Product" },
        new Tag { Id = 36, Name = "Vitamins", Category = "Product" },
        new Tag { Id = 37, Name = "Orthopedic Supports", Category = "Product" },
        new Tag { Id = 38, Name = "First Aid Kit", Category = "Product" },
        new Tag { Id = 39, Name = "Oral Care", Category = "Product" },
        new Tag { Id = 40, Name = "Sanitizers & Hygiene", Category = "Product" },
        new Tag { Id = 41, Name = "Diabetes Care", Category = "Product" },
        new Tag { Id = 42, Name = "Mobility Aids", Category = "Product" },
        new Tag { Id = 43, Name = "Compression Wear", Category = "Product" },
        new Tag { Id = 44, Name = "Sunscreen", Category = "Product" },
        new Tag { Id = 45, Name = "Baby Care Products", Category = "Product" },
        new Tag { Id = 46, Name = "Protein & Nutrition Bars", Category = "Product" },
        new Tag { Id = 47, Name = "Eye Care & Drops", Category = "Product" },
        new Tag { Id = 48, Name = "Diagnostic Devices", Category = "Product" },
        new Tag { Id = 49, Name = "Pain Relief Topical", Category = "Product" },
        new Tag { Id = 50, Name = "Organic / Herbal Oils", Category = "Product" }
    );
        }

    }
}