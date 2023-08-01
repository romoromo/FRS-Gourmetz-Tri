using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace FRS.Migrations
{
    public partial class added_epaper_tables : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "EpaperDevices",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    IsActive = table.Column<bool>(nullable: false),
                    CreatedBy = table.Column<int>(nullable: true),
                    UpdatedBy = table.Column<int>(nullable: true),
                    UpdatedDate = table.Column<DateTime>(nullable: false),
                    CreatedDate = table.Column<DateTime>(nullable: false),
                    device_code = table.Column<string>(nullable: true),
                    device_status = table.Column<int>(nullable: false),
                    device_label = table.Column<string>(nullable: true),
                    mac_address = table.Column<string>(nullable: true),
                    ip_address = table.Column<string>(nullable: true),
                    host_address = table.Column<string>(nullable: true),
                    last_heartbeat = table.Column<DateTime>(nullable: true),
                    location_id = table.Column<long>(nullable: true),
                    location_code = table.Column<string>(nullable: true),
                    epaper_url = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EpaperDevices", x => x.Id);
                    table.ForeignKey(
                        name: "FK_EpaperDevices_User_CreatedBy",
                        column: x => x.CreatedBy,
                        principalTable: "User",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_EpaperDevices_User_UpdatedBy",
                        column: x => x.UpdatedBy,
                        principalTable: "User",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "EpaperTemplates",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    IsActive = table.Column<bool>(nullable: false),
                    CreatedBy = table.Column<int>(nullable: true),
                    UpdatedBy = table.Column<int>(nullable: true),
                    UpdatedDate = table.Column<DateTime>(nullable: false),
                    CreatedDate = table.Column<DateTime>(nullable: false),
                    TemplateBody = table.Column<string>(nullable: true),
                    Name = table.Column<string>(nullable: true),
                    Description = table.Column<string>(nullable: true),
                    ImgUrl = table.Column<string>(nullable: true),
                    DeviceAPIURl = table.Column<string>(nullable: true),
                    DeviceImageAPIUrl = table.Column<string>(nullable: true),
                    IsPostToDevice = table.Column<bool>(nullable: false),
                    IsMapToAPI = table.Column<bool>(nullable: false),
                    MapAPIUrl = table.Column<string>(nullable: true),
                    TemplateType = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EpaperTemplates", x => x.Id);
                    table.ForeignKey(
                        name: "FK_EpaperTemplates_User_CreatedBy",
                        column: x => x.CreatedBy,
                        principalTable: "User",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_EpaperTemplates_User_UpdatedBy",
                        column: x => x.UpdatedBy,
                        principalTable: "User",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "EpaperTemplateLocations",
                columns: table => new
                {
                    LocationId = table.Column<int>(nullable: false),
                    EpaperTemplateId = table.Column<int>(nullable: false),
                    IsActive = table.Column<bool>(nullable: false),
                    CreatedBy = table.Column<int>(nullable: true),
                    UpdatedBy = table.Column<int>(nullable: true),
                    UpdatedDate = table.Column<DateTime>(nullable: false),
                    CreatedDate = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EpaperTemplateLocations", x => new { x.LocationId, x.EpaperTemplateId });
                    table.ForeignKey(
                        name: "FK_EpaperTemplateLocations_User_CreatedBy",
                        column: x => x.CreatedBy,
                        principalTable: "User",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_EpaperTemplateLocations_EpaperTemplates_EpaperTemplateId",
                        column: x => x.EpaperTemplateId,
                        principalTable: "EpaperTemplates",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_EpaperTemplateLocations_Locations_LocationId",
                        column: x => x.LocationId,
                        principalTable: "Locations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_EpaperTemplateLocations_User_UpdatedBy",
                        column: x => x.UpdatedBy,
                        principalTable: "User",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_EpaperDevices_CreatedBy",
                table: "EpaperDevices",
                column: "CreatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_EpaperDevices_UpdatedBy",
                table: "EpaperDevices",
                column: "UpdatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_EpaperTemplateLocations_CreatedBy",
                table: "EpaperTemplateLocations",
                column: "CreatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_EpaperTemplateLocations_EpaperTemplateId",
                table: "EpaperTemplateLocations",
                column: "EpaperTemplateId");

            migrationBuilder.CreateIndex(
                name: "IX_EpaperTemplateLocations_UpdatedBy",
                table: "EpaperTemplateLocations",
                column: "UpdatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_EpaperTemplates_CreatedBy",
                table: "EpaperTemplates",
                column: "CreatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_EpaperTemplates_UpdatedBy",
                table: "EpaperTemplates",
                column: "UpdatedBy");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "EpaperDevices");

            migrationBuilder.DropTable(
                name: "EpaperTemplateLocations");

            migrationBuilder.DropTable(
                name: "EpaperTemplates");
        }
    }
}
