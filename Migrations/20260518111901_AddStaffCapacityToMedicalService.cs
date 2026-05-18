using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ClinicApp.API.Migrations
{
    /// <inheritdoc />
    public partial class AddStaffCapacityToMedicalService : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "AvailableStaffCapacity",
                table: "MedicalServices",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AvailableStaffCapacity",
                table: "MedicalServices");
        }
    }
}
