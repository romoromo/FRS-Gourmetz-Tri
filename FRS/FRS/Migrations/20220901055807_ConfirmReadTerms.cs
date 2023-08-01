using Microsoft.EntityFrameworkCore.Migrations;

namespace FRS.Migrations
{
    public partial class ConfirmReadTerms : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "ConfirmReadTermsConditions",
                table: "User",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "ConsentDataCollection",
                table: "User",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "ReceivePromotionalMaterials",
                table: "User",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ConfirmReadTermsConditions",
                table: "User");

            migrationBuilder.DropColumn(
                name: "ConsentDataCollection",
                table: "User");

            migrationBuilder.DropColumn(
                name: "ReceivePromotionalMaterials",
                table: "User");
        }
    }
}
