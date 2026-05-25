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

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<StaffServices>().HasKey(ss => new { ss.StaffId, ss.MedicalServiceId });
        }

    }
}