using Microsoft.EntityFrameworkCore.Migrations;

namespace FRS.Migrations
{
    public partial class added_new_field_restriction : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsCarriedForward",
                table: "RestrictionTypes",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsDishTypeCustomisation",
                table: "RestrictionTypes",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsMealFiltering",
                table: "RestrictionTypes",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsSpecialCondition",
                table: "RestrictionTypes",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "Sequence",
                table: "RestrictionTypes",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<bool>(
                name: "IsAvailableDateSpecific",
                table: "Restrictions",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsHighPriority",
                table: "Restrictions",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsIncludeHighPriorityFiltering",
                table: "Restrictions",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsMealFiltering",
                table: "Restrictions",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsCarriedForward",
                table: "RestrictionTypes");

            migrationBuilder.DropColumn(
                name: "IsDishTypeCustomisation",
                table: "RestrictionTypes");

            migrationBuilder.DropColumn(
                name: "IsMealFiltering",
                table: "RestrictionTypes");

            migrationBuilder.DropColumn(
                name: "IsSpecialCondition",
                table: "RestrictionTypes");

            migrationBuilder.DropColumn(
                name: "Sequence",
                table: "RestrictionTypes");

            migrationBuilder.DropColumn(
                name: "IsAvailableDateSpecific",
                table: "Restrictions");

            migrationBuilder.DropColumn(
                name: "IsHighPriority",
                table: "Restrictions");

            migrationBuilder.DropColumn(
                name: "IsIncludeHighPriorityFiltering",
                table: "Restrictions");

            migrationBuilder.DropColumn(
                name: "IsMealFiltering",
                table: "Restrictions");
        }
    }
}
