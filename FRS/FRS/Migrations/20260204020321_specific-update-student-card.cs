using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace FRS.Migrations
{
    /// <inheritdoc />
    public partial class specificupdatestudentcard : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "LastUpdateDate",
                table: "StudentCards",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<int>(
                name: "UserUpdateId",
                table: "StudentCards",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_StudentCards_UserUpdateId",
                table: "StudentCards",
                column: "UserUpdateId");

            migrationBuilder.AddForeignKey(
                name: "FK_StudentCards_User_UserUpdateId",
                table: "StudentCards",
                column: "UserUpdateId",
                principalTable: "User",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_StudentCards_User_UserUpdateId",
                table: "StudentCards");

            migrationBuilder.DropIndex(
                name: "IX_StudentCards_UserUpdateId",
                table: "StudentCards");

            migrationBuilder.DropColumn(
                name: "LastUpdateDate",
                table: "StudentCards");

            migrationBuilder.DropColumn(
                name: "UserUpdateId",
                table: "StudentCards");
        }
    }
}
