using Microsoft.EntityFrameworkCore.Migrations;

namespace FRS.Migrations
{
    public partial class added_outletid_classbatch : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "OutletId",
                table: "ClassBatches",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_ClassBatches_OutletId",
                table: "ClassBatches",
                column: "OutletId");

            migrationBuilder.AddForeignKey(
                name: "FK_ClassBatches_Outlets_OutletId",
                table: "ClassBatches",
                column: "OutletId",
                principalTable: "Outlets",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ClassBatches_Outlets_OutletId",
                table: "ClassBatches");

            migrationBuilder.DropIndex(
                name: "IX_ClassBatches_OutletId",
                table: "ClassBatches");

            migrationBuilder.DropColumn(
                name: "OutletId",
                table: "ClassBatches");
        }
    }
}
