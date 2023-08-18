using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace FRS.Migrations
{
    public partial class updateportalcontent : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "EffectiveDate",
                table: "OrderPortalContents",
                newName: "EffectiveStartDate");

            migrationBuilder.AddColumn<DateTime>(
                name: "EffectiveEndDate",
                table: "OrderPortalContents",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "EffectiveEndDate",
                table: "OrderPortalContents");

            migrationBuilder.RenameColumn(
                name: "EffectiveStartDate",
                table: "OrderPortalContents",
                newName: "EffectiveDate");
        }
    }
}
