using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace FRS.Migrations
{
    public partial class added_location_image_ref : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "FacePlateNumber",
                table: "Locations",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "LocationImageReferences",
                columns: table => new
                {
                    LocationId = table.Column<int>(nullable: false),
                    FileId = table.Column<int>(nullable: false),
                    IsActive = table.Column<bool>(nullable: false),
                    CreatedBy = table.Column<int>(nullable: true),
                    UpdatedBy = table.Column<int>(nullable: true),
                    UpdatedDate = table.Column<DateTime>(nullable: false),
                    CreatedDate = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LocationImageReferences", x => new { x.LocationId, x.FileId });
                    table.ForeignKey(
                        name: "FK_LocationImageReferences_User_CreatedBy",
                        column: x => x.CreatedBy,
                        principalTable: "User",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_LocationImageReferences_Files_FileId",
                        column: x => x.FileId,
                        principalTable: "Files",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_LocationImageReferences_Locations_LocationId",
                        column: x => x.LocationId,
                        principalTable: "Locations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_LocationImageReferences_User_UpdatedBy",
                        column: x => x.UpdatedBy,
                        principalTable: "User",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Devices_InstitutionId_Code_IsActive",
                table: "Devices",
                columns: new[] { "InstitutionId", "Code", "IsActive" },
                unique: true,
                filter: "[InstitutionId] IS NOT NULL AND [Code] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_LocationImageReferences_CreatedBy",
                table: "LocationImageReferences",
                column: "CreatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_LocationImageReferences_FileId",
                table: "LocationImageReferences",
                column: "FileId");

            migrationBuilder.CreateIndex(
                name: "IX_LocationImageReferences_UpdatedBy",
                table: "LocationImageReferences",
                column: "UpdatedBy");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "LocationImageReferences");

            migrationBuilder.DropIndex(
                name: "IX_Devices_InstitutionId_Code_IsActive",
                table: "Devices");

            migrationBuilder.DropColumn(
                name: "FacePlateNumber",
                table: "Locations");
        }
    }
}
