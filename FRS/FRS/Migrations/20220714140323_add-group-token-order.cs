using Microsoft.EntityFrameworkCore.Migrations;

namespace FRS.Migrations
{
    public partial class addgrouptokenorder : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TokenOrders_Students_ProfileId",
                table: "TokenOrders");

            migrationBuilder.AlterColumn<int>(
                name: "ProfileId",
                table: "TokenOrders",
                nullable: true,
                oldClrType: typeof(int));

            migrationBuilder.AddColumn<int>(
                name: "StudentGroupId",
                table: "TokenOrders",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_TokenOrders_StudentGroupId",
                table: "TokenOrders",
                column: "StudentGroupId");

            migrationBuilder.AddForeignKey(
                name: "FK_TokenOrders_Students_ProfileId",
                table: "TokenOrders",
                column: "ProfileId",
                principalTable: "Students",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_TokenOrders_StudentGroups_StudentGroupId",
                table: "TokenOrders",
                column: "StudentGroupId",
                principalTable: "StudentGroups",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TokenOrders_Students_ProfileId",
                table: "TokenOrders");

            migrationBuilder.DropForeignKey(
                name: "FK_TokenOrders_StudentGroups_StudentGroupId",
                table: "TokenOrders");

            migrationBuilder.DropIndex(
                name: "IX_TokenOrders_StudentGroupId",
                table: "TokenOrders");

            migrationBuilder.DropColumn(
                name: "StudentGroupId",
                table: "TokenOrders");

            migrationBuilder.AlterColumn<int>(
                name: "ProfileId",
                table: "TokenOrders",
                nullable: false,
                oldClrType: typeof(int),
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_TokenOrders_Students_ProfileId",
                table: "TokenOrders",
                column: "ProfileId",
                principalTable: "Students",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
