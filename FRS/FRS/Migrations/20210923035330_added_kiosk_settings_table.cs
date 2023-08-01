using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace FRS.Migrations
{
    public partial class added_kiosk_settings_table : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "KioskSettings",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    IsActive = table.Column<bool>(nullable: false),
                    CreatedBy = table.Column<int>(nullable: true),
                    UpdatedBy = table.Column<int>(nullable: true),
                    UpdatedDate = table.Column<DateTime>(nullable: false),
                    CreatedDate = table.Column<DateTime>(nullable: false),
                    label = table.Column<string>(nullable: true),
                    screen_save_time = table.Column<int>(nullable: false),
                    ss_playlist_id = table.Column<int>(nullable: true),
                    ss_playlistId = table.Column<int>(nullable: true),
                    bannerId = table.Column<int>(nullable: true),
                    bannerImageId = table.Column<int>(nullable: true),
                    top_banner_id = table.Column<int>(nullable: true),
                    top_bannerId = table.Column<int>(nullable: true),
                    bottom_banner_id = table.Column<int>(nullable: true),
                    bottom_bannerId = table.Column<int>(nullable: true),
                    route_weight = table.Column<float>(nullable: true),
                    route_speed = table.Column<float>(nullable: true),
                    route_color = table.Column<string>(nullable: true),
                    jan = table.Column<string>(nullable: true),
                    feb = table.Column<string>(nullable: true),
                    mar = table.Column<string>(nullable: true),
                    apr = table.Column<string>(nullable: true),
                    may = table.Column<string>(nullable: true),
                    jun = table.Column<string>(nullable: true),
                    jul = table.Column<string>(nullable: true),
                    aug = table.Column<string>(nullable: true),
                    sep = table.Column<string>(nullable: true),
                    oct = table.Column<string>(nullable: true),
                    nov = table.Column<string>(nullable: true),
                    dec = table.Column<string>(nullable: true),
                    dir_color = table.Column<string>(nullable: true),
                    dir_font_color = table.Column<string>(nullable: true),
                    dir_font_type = table.Column<string>(nullable: true),
                    def_event_id = table.Column<int>(nullable: true),
                    def_eventId = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_KioskSettings", x => x.Id);
                    table.ForeignKey(
                        name: "FK_KioskSettings_User_CreatedBy",
                        column: x => x.CreatedBy,
                        principalTable: "User",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_KioskSettings_User_UpdatedBy",
                        column: x => x.UpdatedBy,
                        principalTable: "User",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_KioskSettings_ImageFiles_bannerImageId",
                        column: x => x.bannerImageId,
                        principalTable: "ImageFiles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_KioskSettings_Playlists_bottom_bannerId",
                        column: x => x.bottom_bannerId,
                        principalTable: "Playlists",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_KioskSettings_Playlists_def_eventId",
                        column: x => x.def_eventId,
                        principalTable: "Playlists",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_KioskSettings_Playlists_ss_playlistId",
                        column: x => x.ss_playlistId,
                        principalTable: "Playlists",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_KioskSettings_Playlists_top_bannerId",
                        column: x => x.top_bannerId,
                        principalTable: "Playlists",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_KioskSettings_CreatedBy",
                table: "KioskSettings",
                column: "CreatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_KioskSettings_UpdatedBy",
                table: "KioskSettings",
                column: "UpdatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_KioskSettings_bannerImageId",
                table: "KioskSettings",
                column: "bannerImageId");

            migrationBuilder.CreateIndex(
                name: "IX_KioskSettings_bottom_bannerId",
                table: "KioskSettings",
                column: "bottom_bannerId");

            migrationBuilder.CreateIndex(
                name: "IX_KioskSettings_def_eventId",
                table: "KioskSettings",
                column: "def_eventId");

            migrationBuilder.CreateIndex(
                name: "IX_KioskSettings_ss_playlistId",
                table: "KioskSettings",
                column: "ss_playlistId");

            migrationBuilder.CreateIndex(
                name: "IX_KioskSettings_top_bannerId",
                table: "KioskSettings",
                column: "top_bannerId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "KioskSettings");
        }
    }
}
