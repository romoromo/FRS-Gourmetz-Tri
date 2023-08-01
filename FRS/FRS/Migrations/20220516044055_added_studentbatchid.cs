using Microsoft.EntityFrameworkCore.Migrations;

namespace FRS.Migrations
{
    public partial class added_studentbatchid : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Classes_ClassBatches_ClassBatchId",
                table: "Classes");

            migrationBuilder.DropIndex(
                name: "IX_Classes_ClassBatchId",
                table: "Classes");

            migrationBuilder.DropColumn(
                name: "ClassBatchId",
                table: "Classes");

            migrationBuilder.AddColumn<int>(
                name: "ClassBatchId",
                table: "Students",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Students_ClassBatchId",
                table: "Students",
                column: "ClassBatchId");

            migrationBuilder.AddForeignKey(
                name: "FK_Students_ClassBatches_ClassBatchId",
                table: "Students",
                column: "ClassBatchId",
                principalTable: "ClassBatches",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Students_ClassBatches_ClassBatchId",
                table: "Students");

            migrationBuilder.DropIndex(
                name: "IX_Students_ClassBatchId",
                table: "Students");

            migrationBuilder.DropColumn(
                name: "ClassBatchId",
                table: "Students");

            migrationBuilder.AddColumn<int>(
                name: "ClassBatchId",
                table: "Classes",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Classes_ClassBatchId",
                table: "Classes",
                column: "ClassBatchId");

            migrationBuilder.AddForeignKey(
                name: "FK_Classes_ClassBatches_ClassBatchId",
                table: "Classes",
                column: "ClassBatchId",
                principalTable: "ClassBatches",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
