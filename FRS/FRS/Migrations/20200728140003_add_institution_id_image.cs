using Microsoft.EntityFrameworkCore.Migrations;

namespace FRS.Migrations
{
    public partial class add_institution_id_image : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ImageName",
                table: "ImageFiles");

            migrationBuilder.AddColumn<int>(
                name: "InstitutionId",
                table: "ImageFiles",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_ImageFiles_InstitutionId",
                table: "ImageFiles",
                column: "InstitutionId");

            migrationBuilder.AddForeignKey(
                name: "FK_ImageFiles_Institutions_InstitutionId",
                table: "ImageFiles",
                column: "InstitutionId",
                principalTable: "Institutions",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ImageFiles_Institutions_InstitutionId",
                table: "ImageFiles");

            migrationBuilder.DropIndex(
                name: "IX_ImageFiles_InstitutionId",
                table: "ImageFiles");

            migrationBuilder.DropColumn(
                name: "InstitutionId",
                table: "ImageFiles");

            migrationBuilder.AddColumn<string>(
                name: "ImageName",
                table: "ImageFiles",
                nullable: true);
        }
    }
}
