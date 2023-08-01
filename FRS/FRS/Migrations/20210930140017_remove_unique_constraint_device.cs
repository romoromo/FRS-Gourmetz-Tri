using Microsoft.EntityFrameworkCore.Migrations;

namespace FRS.Migrations
{
    public partial class remove_unique_constraint_device : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Devices_InstitutionId_Code_IsActive",
                table: "Devices");

            migrationBuilder.AddColumn<string>(
                name: "Remarks",
                table: "LocationImageReferences",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Remarks",
                table: "LocationImageReferences");

            migrationBuilder.CreateIndex(
                name: "IX_Devices_InstitutionId_Code_IsActive",
                table: "Devices",
                columns: new[] { "InstitutionId", "Code", "IsActive" },
                unique: true,
                filter: "[InstitutionId] IS NOT NULL AND [Code] IS NOT NULL");
        }
    }
}
