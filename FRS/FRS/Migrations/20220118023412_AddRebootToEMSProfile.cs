using Microsoft.EntityFrameworkCore.Migrations;

namespace FRS.Migrations
{
    public partial class AddRebootToEMSProfile : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "reboot",
                table: "EmsProfiles",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "reboot",
                table: "EmsProfiles");
        }
    }
}
