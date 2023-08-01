using Microsoft.EntityFrameworkCore.Migrations;

namespace FRS.Migrations
{
    public partial class addDeviceIdentifier : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DeviceId",
                table: "QueueTableMap");

            migrationBuilder.AddColumn<int>(
                name: "DeviceIdentifier",
                table: "QueueTableMap",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_QueueTableMap_DeviceIdentifier",
                table: "QueueTableMap",
                column: "DeviceIdentifier");

            migrationBuilder.AddForeignKey(
                name: "FK_QueueTableMap_Devices_DeviceIdentifier",
                table: "QueueTableMap",
                column: "DeviceIdentifier",
                principalTable: "Devices",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_QueueTableMap_Devices_DeviceIdentifier",
                table: "QueueTableMap");

            migrationBuilder.DropIndex(
                name: "IX_QueueTableMap_DeviceIdentifier",
                table: "QueueTableMap");

            migrationBuilder.DropColumn(
                name: "DeviceIdentifier",
                table: "QueueTableMap");

            migrationBuilder.AddColumn<string>(
                name: "DeviceId",
                table: "QueueTableMap",
                nullable: true);
        }
    }
}
