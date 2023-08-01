using Microsoft.EntityFrameworkCore.Migrations;

namespace FRS.Migrations
{
    public partial class added_new_device_columns : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "CommunicatorMenuId",
                table: "Devices",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ModuleId",
                table: "Devices",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Devices_ModuleId",
                table: "Devices",
                column: "ModuleId");

            migrationBuilder.AddForeignKey(
                name: "FK_Devices_Modules_ModuleId",
                table: "Devices",
                column: "ModuleId",
                principalTable: "Modules",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Devices_Modules_ModuleId",
                table: "Devices");

            migrationBuilder.DropIndex(
                name: "IX_Devices_ModuleId",
                table: "Devices");

            migrationBuilder.DropColumn(
                name: "CommunicatorMenuId",
                table: "Devices");

            migrationBuilder.DropColumn(
                name: "ModuleId",
                table: "Devices");
        }
    }
}
