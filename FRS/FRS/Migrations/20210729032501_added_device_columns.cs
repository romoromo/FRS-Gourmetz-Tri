using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace FRS.Migrations
{
    public partial class added_device_columns : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Devices_Institutions_InstitutionId",
                table: "Devices");

            migrationBuilder.DropForeignKey(
                name: "FK_Devices_Locations_LocationId",
                table: "Devices");

            migrationBuilder.DropIndex(
                name: "IX_Devices_InstitutionId",
                table: "Devices");

            migrationBuilder.DropIndex(
                name: "IX_Devices_LocationId",
                table: "Devices");

            migrationBuilder.AddColumn<int>(
                name: "Brightness",
                table: "Devices",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "InstitutionName",
                table: "Devices",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "LocationName",
                table: "Devices",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Resolution",
                table: "Devices",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Rotation",
                table: "Devices",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Volume",
                table: "Devices",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ems_id",
                table: "Devices",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "isScreenOn",
                table: "Devices",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "media_id",
                table: "Devices",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "monitorStatus",
                table: "Devices",
                nullable: false,
                defaultValue: false);

            migrationBuilder.CreateTable(
                name: "ModuleParameters",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    IsActive = table.Column<bool>(nullable: false),
                    CreatedBy = table.Column<int>(nullable: true),
                    UpdatedBy = table.Column<int>(nullable: true),
                    UpdatedDate = table.Column<DateTime>(nullable: false),
                    CreatedDate = table.Column<DateTime>(nullable: false),
                    Name = table.Column<string>(nullable: true),
                    Url = table.Column<string>(nullable: true),
                    ModuleId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ModuleParameters", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ModuleParameters_User_CreatedBy",
                        column: x => x.CreatedBy,
                        principalTable: "User",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ModuleParameters_Modules_ModuleId",
                        column: x => x.ModuleId,
                        principalTable: "Modules",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ModuleParameters_User_UpdatedBy",
                        column: x => x.UpdatedBy,
                        principalTable: "User",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ModuleParameters_CreatedBy",
                table: "ModuleParameters",
                column: "CreatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_ModuleParameters_ModuleId",
                table: "ModuleParameters",
                column: "ModuleId");

            migrationBuilder.CreateIndex(
                name: "IX_ModuleParameters_UpdatedBy",
                table: "ModuleParameters",
                column: "UpdatedBy");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ModuleParameters");

            migrationBuilder.DropColumn(
                name: "Brightness",
                table: "Devices");

            migrationBuilder.DropColumn(
                name: "InstitutionName",
                table: "Devices");

            migrationBuilder.DropColumn(
                name: "LocationName",
                table: "Devices");

            migrationBuilder.DropColumn(
                name: "Resolution",
                table: "Devices");

            migrationBuilder.DropColumn(
                name: "Rotation",
                table: "Devices");

            migrationBuilder.DropColumn(
                name: "Volume",
                table: "Devices");

            migrationBuilder.DropColumn(
                name: "ems_id",
                table: "Devices");

            migrationBuilder.DropColumn(
                name: "isScreenOn",
                table: "Devices");

            migrationBuilder.DropColumn(
                name: "media_id",
                table: "Devices");

            migrationBuilder.DropColumn(
                name: "monitorStatus",
                table: "Devices");

            migrationBuilder.CreateIndex(
                name: "IX_Devices_InstitutionId",
                table: "Devices",
                column: "InstitutionId");

            migrationBuilder.CreateIndex(
                name: "IX_Devices_LocationId",
                table: "Devices",
                column: "LocationId");

            migrationBuilder.AddForeignKey(
                name: "FK_Devices_Institutions_InstitutionId",
                table: "Devices",
                column: "InstitutionId",
                principalTable: "Institutions",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Devices_Locations_LocationId",
                table: "Devices",
                column: "LocationId",
                principalTable: "Locations",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
