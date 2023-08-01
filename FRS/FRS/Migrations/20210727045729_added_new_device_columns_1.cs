using Microsoft.EntityFrameworkCore.Migrations;

namespace FRS.Migrations
{
    public partial class added_new_device_columns_1 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "serialno",
                table: "Devices",
                newName: "device_label");

            migrationBuilder.AddColumn<int>(
                name: "PIBTemplateId",
                table: "Devices",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PIBTemplateId",
                table: "Devices");

            migrationBuilder.RenameColumn(
                name: "device_label",
                table: "Devices",
                newName: "serialno");
        }
    }
}
