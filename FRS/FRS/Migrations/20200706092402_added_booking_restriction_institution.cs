using Microsoft.EntityFrameworkCore.Migrations;

namespace FRS.Migrations
{
    public partial class added_booking_restriction_institution : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "UnitNumber",
                table: "User",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsRestrictDuplicateBooking",
                table: "Institutions",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "UnitNumber",
                table: "User");

            migrationBuilder.DropColumn(
                name: "IsRestrictDuplicateBooking",
                table: "Institutions");
        }
    }
}
