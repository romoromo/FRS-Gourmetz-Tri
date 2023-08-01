using Microsoft.EntityFrameworkCore.Migrations;

namespace FRS.Migrations
{
    public partial class changed_outlet_outletprofileassociation : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_CatererOutlets",
                table: "CatererOutlets");

            migrationBuilder.AddColumn<int>(
                name: "OutletProfileId",
                table: "CatererOutlets",
                nullable: false,
                defaultValue: 1);

            migrationBuilder.AddPrimaryKey(
                name: "PK_CatererOutlets",
                table: "CatererOutlets",
                columns: new[] { "CatererInfoId", "OutletId", "OutletProfileId" });

            migrationBuilder.CreateIndex(
                name: "IX_CatererOutlets_OutletProfileId",
                table: "CatererOutlets",
                column: "OutletProfileId");

            migrationBuilder.AddForeignKey(
                name: "FK_CatererOutlets_OutletProfiles_OutletProfileId",
                table: "CatererOutlets",
                column: "OutletProfileId",
                principalTable: "OutletProfiles",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CatererOutlets_OutletProfiles_OutletProfileId",
                table: "CatererOutlets");

            migrationBuilder.DropPrimaryKey(
                name: "PK_CatererOutlets",
                table: "CatererOutlets");

            migrationBuilder.DropIndex(
                name: "IX_CatererOutlets_OutletProfileId",
                table: "CatererOutlets");

            migrationBuilder.DropColumn(
                name: "OutletProfileId",
                table: "CatererOutlets");

            migrationBuilder.AddPrimaryKey(
                name: "PK_CatererOutlets",
                table: "CatererOutlets",
                columns: new[] { "CatererInfoId", "OutletId" });
        }
    }
}
