using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace FRS.Migrations
{
    public partial class add_fields_image_reference : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "ReferenceDate",
                table: "LocationImageReferences",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Type",
                table: "LocationImageReferences",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ReferenceDate",
                table: "LocationImageReferences");

            migrationBuilder.DropColumn(
                name: "Type",
                table: "LocationImageReferences");
        }
    }
}
