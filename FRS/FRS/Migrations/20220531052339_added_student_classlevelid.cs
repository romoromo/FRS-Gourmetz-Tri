using Microsoft.EntityFrameworkCore.Migrations;

namespace FRS.Migrations
{
    public partial class added_student_classlevelid : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ClassLevelId",
                table: "Students",
                nullable: true);

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
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
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
        }
    }
}
