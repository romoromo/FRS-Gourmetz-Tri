using Microsoft.EntityFrameworkCore.Migrations;

namespace FRS.Migrations
{
    public partial class rename_student_account : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Staffs_Departments_DepartmentId",
                table: "Staffs");

            migrationBuilder.DropForeignKey(
                name: "FK_StudentAccount_Students_StudentId",
                table: "StudentAccount");

            migrationBuilder.DropForeignKey(
                name: "FK_StudentAccount_User_UserId",
                table: "StudentAccount");

            migrationBuilder.DropPrimaryKey(
                name: "PK_StudentAccount",
                table: "StudentAccount");

            migrationBuilder.RenameTable(
                name: "StudentAccount",
                newName: "StudentAccounts");

            migrationBuilder.RenameIndex(
                name: "IX_StudentAccount_UserId",
                table: "StudentAccounts",
                newName: "IX_StudentAccounts_UserId");

            migrationBuilder.RenameIndex(
                name: "IX_StudentAccount_StudentId",
                table: "StudentAccounts",
                newName: "IX_StudentAccounts_StudentId");

            migrationBuilder.AlterColumn<int>(
                name: "DepartmentId",
                table: "Staffs",
                nullable: true,
                oldClrType: typeof(int));

            migrationBuilder.AddPrimaryKey(
                name: "PK_StudentAccounts",
                table: "StudentAccounts",
                columns: new[] { "StudentId", "UserId" });

            migrationBuilder.AddForeignKey(
                name: "FK_Staffs_Departments_DepartmentId",
                table: "Staffs",
                column: "DepartmentId",
                principalTable: "Departments",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_StudentAccounts_Students_StudentId",
                table: "StudentAccounts",
                column: "StudentId",
                principalTable: "Students",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_StudentAccounts_User_UserId",
                table: "StudentAccounts",
                column: "UserId",
                principalTable: "User",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Staffs_Departments_DepartmentId",
                table: "Staffs");

            migrationBuilder.DropForeignKey(
                name: "FK_StudentAccounts_Students_StudentId",
                table: "StudentAccounts");

            migrationBuilder.DropForeignKey(
                name: "FK_StudentAccounts_User_UserId",
                table: "StudentAccounts");

            migrationBuilder.DropPrimaryKey(
                name: "PK_StudentAccounts",
                table: "StudentAccounts");

            migrationBuilder.RenameTable(
                name: "StudentAccounts",
                newName: "StudentAccount");

            migrationBuilder.RenameIndex(
                name: "IX_StudentAccounts_UserId",
                table: "StudentAccount",
                newName: "IX_StudentAccount_UserId");

            migrationBuilder.RenameIndex(
                name: "IX_StudentAccounts_StudentId",
                table: "StudentAccount",
                newName: "IX_StudentAccount_StudentId");

            migrationBuilder.AlterColumn<int>(
                name: "DepartmentId",
                table: "Staffs",
                nullable: false,
                oldClrType: typeof(int),
                oldNullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_StudentAccount",
                table: "StudentAccount",
                columns: new[] { "StudentId", "UserId" });

            migrationBuilder.AddForeignKey(
                name: "FK_Staffs_Departments_DepartmentId",
                table: "Staffs",
                column: "DepartmentId",
                principalTable: "Departments",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_StudentAccount_Students_StudentId",
                table: "StudentAccount",
                column: "StudentId",
                principalTable: "Students",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_StudentAccount_User_UserId",
                table: "StudentAccount",
                column: "UserId",
                principalTable: "User",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
