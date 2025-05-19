using Microsoft.EntityFrameworkCore.Migrations;

//#nullable disable

namespace FRS.Migrations
{
    /// <inheritdoc />
    public partial class AddIndexForStudentVoucher : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Status",
                table: "StudentVouchers",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_StudentVouchers_Status",
                table: "StudentVouchers",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_StudentVouchers_StudentId_VoucherId",
                table: "StudentVouchers",
                columns: new[] { "StudentId", "VoucherId" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_StudentVouchers_Status",
                table: "StudentVouchers");

            migrationBuilder.DropIndex(
                name: "IX_StudentVouchers_StudentId_VoucherId",
                table: "StudentVouchers");

            migrationBuilder.AlterColumn<string>(
                name: "Status",
                table: "StudentVouchers",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50,
                oldNullable: true);
        }
    }
}
