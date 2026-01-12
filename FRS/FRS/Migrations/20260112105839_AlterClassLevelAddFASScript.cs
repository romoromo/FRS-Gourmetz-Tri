using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace FRS.Migrations
{
    /// <inheritdoc />
    public partial class AlterClassLevelAddFASScript : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<double>(
                name: "Amount",
                table: "ClassLevels",
                type: "float",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Day",
                table: "ClassLevels",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "FASRechargeable",
                table: "ClassLevels",
                type: "bit",
                nullable: true);

            migrationBuilder.AddColumn<TimeSpan>(
                name: "Time",
                table: "ClassLevels",
                type: "time",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Amount",
                table: "ClassLevels");

            migrationBuilder.DropColumn(
                name: "Day",
                table: "ClassLevels");

            migrationBuilder.DropColumn(
                name: "FASRechargeable",
                table: "ClassLevels");

            migrationBuilder.DropColumn(
                name: "Time",
                table: "ClassLevels");
        }
    }
}
