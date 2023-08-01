using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace FRS.Migrations
{
    public partial class added_new_fields_device : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "device_status",
                table: "Devices",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<DateTime>(
                name: "last_heartbeat",
                table: "Devices",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "module_path",
                table: "Devices",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "module_path_value",
                table: "Devices",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "device_status",
                table: "Devices");

            migrationBuilder.DropColumn(
                name: "last_heartbeat",
                table: "Devices");

            migrationBuilder.DropColumn(
                name: "module_path",
                table: "Devices");

            migrationBuilder.DropColumn(
                name: "module_path_value",
                table: "Devices");
        }
    }
}
