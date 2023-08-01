using Microsoft.EntityFrameworkCore.Migrations;

namespace FRS.Migrations
{
    public partial class addPUblicationId : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "PublicationId",
                table: "Devices",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Devices_PublicationId",
                table: "Devices",
                column: "PublicationId");

            migrationBuilder.AddForeignKey(
                name: "FK_Devices_SignagePublications_PublicationId",
                table: "Devices",
                column: "PublicationId",
                principalTable: "SignagePublications",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Devices_SignagePublications_PublicationId",
                table: "Devices");

            migrationBuilder.DropIndex(
                name: "IX_Devices_PublicationId",
                table: "Devices");

            migrationBuilder.DropColumn(
                name: "PublicationId",
                table: "Devices");
        }
    }
}
