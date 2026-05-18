using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ClinicApp.API.Migrations
{
    /// <inheritdoc />
    public partial class AddClinicWorkingHours : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<TimeSpan>(
                name: "ClosingTime",
                table: "Clinics",
                type: "time(6)",
                nullable: false,
                defaultValue: new TimeSpan(0, 0, 0, 0, 0));

            migrationBuilder.AddColumn<TimeSpan>(
                name: "OpeningTime",
                table: "Clinics",
                type: "time(6)",
                nullable: false,
                defaultValue: new TimeSpan(0, 0, 0, 0, 0));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ClosingTime",
                table: "Clinics");

            migrationBuilder.DropColumn(
                name: "OpeningTime",
                table: "Clinics");
        }
    }
}
