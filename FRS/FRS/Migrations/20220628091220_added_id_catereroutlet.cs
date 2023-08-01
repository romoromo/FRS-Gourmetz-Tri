using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace FRS.Migrations
{
    public partial class added_id_catereroutlet : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CatererOutlets_OutletProfiles_OutletProfileId",
                table: "CatererOutlets");

            migrationBuilder.DropPrimaryKey(
                name: "PK_CatererOutlets",
                table: "CatererOutlets");

            migrationBuilder.AlterColumn<int>(
                name: "OutletProfileId",
                table: "CatererOutlets",
                nullable: true,
                oldClrType: typeof(int));

            migrationBuilder.AddColumn<int>(
                name: "Id",
                table: "CatererOutlets",
                nullable: false,
                defaultValue: 0)
                .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            migrationBuilder.AddPrimaryKey(
                name: "PK_CatererOutlets",
                table: "CatererOutlets",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_CatererOutlets_CatererInfoId",
                table: "CatererOutlets",
                column: "CatererInfoId");

            migrationBuilder.AddForeignKey(
                name: "FK_CatererOutlets_OutletProfiles_OutletProfileId",
                table: "CatererOutlets",
                column: "OutletProfileId",
                principalTable: "OutletProfiles",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
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
                name: "IX_CatererOutlets_CatererInfoId",
                table: "CatererOutlets");

            migrationBuilder.DropColumn(
                name: "Id",
                table: "CatererOutlets");

            migrationBuilder.AlterColumn<int>(
                name: "OutletProfileId",
                table: "CatererOutlets",
                nullable: false,
                oldClrType: typeof(int),
                oldNullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_CatererOutlets",
                table: "CatererOutlets",
                columns: new[] { "CatererInfoId", "OutletId", "OutletProfileId" });

            migrationBuilder.AddForeignKey(
                name: "FK_CatererOutlets_OutletProfiles_OutletProfileId",
                table: "CatererOutlets",
                column: "OutletProfileId",
                principalTable: "OutletProfiles",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
