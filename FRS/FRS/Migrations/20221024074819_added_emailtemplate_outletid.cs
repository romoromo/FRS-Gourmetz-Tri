using Microsoft.EntityFrameworkCore.Migrations;

namespace FRS.Migrations
{
    public partial class added_emailtemplate_outletid : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "OutletId",
                table: "EmailTemplates",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_EmailTemplates_OutletId",
                table: "EmailTemplates",
                column: "OutletId");

            migrationBuilder.AddForeignKey(
                name: "FK_EmailTemplates_Outlets_OutletId",
                table: "EmailTemplates",
                column: "OutletId",
                principalTable: "Outlets",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_EmailTemplates_Outlets_OutletId",
                table: "EmailTemplates");

            migrationBuilder.DropIndex(
                name: "IX_EmailTemplates_OutletId",
                table: "EmailTemplates");

            migrationBuilder.DropColumn(
                name: "OutletId",
                table: "EmailTemplates");
        }
    }
}
