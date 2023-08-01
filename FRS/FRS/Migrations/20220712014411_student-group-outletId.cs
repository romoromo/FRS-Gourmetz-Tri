using Microsoft.EntityFrameworkCore.Migrations;

namespace FRS.Migrations
{
    public partial class studentgroupoutletId : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "OutletId",
                table: "StudentGroups",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_StudentGroups_OutletId",
                table: "StudentGroups",
                column: "OutletId");

            migrationBuilder.AddForeignKey(
                name: "FK_StudentGroups_Outlets_OutletId",
                table: "StudentGroups",
                column: "OutletId",
                principalTable: "Outlets",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_StudentGroups_Outlets_OutletId",
                table: "StudentGroups");

            migrationBuilder.DropIndex(
                name: "IX_StudentGroups_OutletId",
                table: "StudentGroups");

            migrationBuilder.DropColumn(
                name: "OutletId",
                table: "StudentGroups");
        }
    }
}
