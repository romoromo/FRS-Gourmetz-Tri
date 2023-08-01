using Microsoft.EntityFrameworkCore.Migrations;

namespace FRS.Migrations
{
    public partial class added_classlevelid_in_class : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Students_ClassLevels_ClassLevelId",
                table: "Students");

            migrationBuilder.DropIndex(
                name: "IX_Students_ClassLevelId",
                table: "Students");

            migrationBuilder.DropColumn(
                name: "ClassLevelId",
                table: "Students");

            migrationBuilder.AddColumn<int>(
                name: "ClassLevelId",
                table: "Classes",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Classes_ClassLevelId",
                table: "Classes",
                column: "ClassLevelId");

            migrationBuilder.AddForeignKey(
                name: "FK_Classes_ClassLevels_ClassLevelId",
                table: "Classes",
                column: "ClassLevelId",
                principalTable: "ClassLevels",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Classes_ClassLevels_ClassLevelId",
                table: "Classes");

            migrationBuilder.DropIndex(
                name: "IX_Classes_ClassLevelId",
                table: "Classes");

            migrationBuilder.DropColumn(
                name: "ClassLevelId",
                table: "Classes");

            migrationBuilder.AddColumn<int>(
                name: "ClassLevelId",
                table: "Students",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Students_ClassLevelId",
                table: "Students",
                column: "ClassLevelId");

            migrationBuilder.AddForeignKey(
                name: "FK_Students_ClassLevels_ClassLevelId",
                table: "Students",
                column: "ClassLevelId",
                principalTable: "ClassLevels",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
