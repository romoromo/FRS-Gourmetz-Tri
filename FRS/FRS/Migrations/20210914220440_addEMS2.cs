using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace FRS.Migrations
{
    public partial class addEMS2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Devices_Ems_ems_id",
                table: "Devices");

            migrationBuilder.DropForeignKey(
                name: "FK_Ems_User_CreatedBy",
                table: "Ems");

            migrationBuilder.DropForeignKey(
                name: "FK_Ems_Institutions_InstitutionId",
                table: "Ems");

            migrationBuilder.DropForeignKey(
                name: "FK_Ems_User_UpdatedBy",
                table: "Ems");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Ems",
                table: "Ems");

            migrationBuilder.RenameTable(
                name: "Ems",
                newName: "Emses");

            migrationBuilder.RenameIndex(
                name: "IX_Ems_UpdatedBy",
                table: "Emses",
                newName: "IX_Emses_UpdatedBy");

            migrationBuilder.RenameIndex(
                name: "IX_Ems_InstitutionId",
                table: "Emses",
                newName: "IX_Emses_InstitutionId");

            migrationBuilder.RenameIndex(
                name: "IX_Ems_CreatedBy",
                table: "Emses",
                newName: "IX_Emses_CreatedBy");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Emses",
                table: "Emses",
                column: "Id");

            migrationBuilder.CreateTable(
                name: "EmsProfiles",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    IsActive = table.Column<bool>(nullable: false),
                    CreatedBy = table.Column<int>(nullable: true),
                    UpdatedBy = table.Column<int>(nullable: true),
                    UpdatedDate = table.Column<DateTime>(nullable: false),
                    CreatedDate = table.Column<DateTime>(nullable: false),
                    Code = table.Column<string>(nullable: true),
                    Label = table.Column<string>(nullable: true),
                    ScreenStatus = table.Column<int>(nullable: false),
                    Rotation = table.Column<string>(nullable: true),
                    Resolution = table.Column<string>(nullable: true),
                    Brightness = table.Column<int>(nullable: true),
                    Volume = table.Column<int>(nullable: true),
                    module_path = table.Column<string>(nullable: true),
                    module_path_value = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EmsProfiles", x => x.Id);
                    table.ForeignKey(
                        name: "FK_EmsProfiles_User_CreatedBy",
                        column: x => x.CreatedBy,
                        principalTable: "User",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_EmsProfiles_User_UpdatedBy",
                        column: x => x.UpdatedBy,
                        principalTable: "User",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "EmsSchedules",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    IsActive = table.Column<bool>(nullable: false),
                    CreatedBy = table.Column<int>(nullable: true),
                    UpdatedBy = table.Column<int>(nullable: true),
                    UpdatedDate = table.Column<DateTime>(nullable: false),
                    CreatedDate = table.Column<DateTime>(nullable: false),
                    Label = table.Column<string>(nullable: true),
                    LocationId = table.Column<long>(nullable: true),
                    EmsProfileId = table.Column<int>(nullable: true),
                    EmsGroupId = table.Column<int>(nullable: true),
                    EffectiveDate = table.Column<DateTime>(nullable: true),
                    IneffectiveDate = table.Column<DateTime>(nullable: true),
                    StartTime = table.Column<DateTime>(nullable: true),
                    EndTime = table.Column<DateTime>(nullable: true),
                    Monday = table.Column<bool>(nullable: false),
                    Tuesday = table.Column<bool>(nullable: false),
                    Wednesday = table.Column<bool>(nullable: false),
                    Thursday = table.Column<bool>(nullable: false),
                    Friday = table.Column<bool>(nullable: false),
                    Saturday = table.Column<bool>(nullable: false),
                    Sunday = table.Column<bool>(nullable: false),
                    ExceptHoliday = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EmsSchedules", x => x.Id);
                    table.ForeignKey(
                        name: "FK_EmsSchedules_User_CreatedBy",
                        column: x => x.CreatedBy,
                        principalTable: "User",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_EmsSchedules_Emses_EmsGroupId",
                        column: x => x.EmsGroupId,
                        principalTable: "Emses",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_EmsSchedules_EmsProfiles_EmsProfileId",
                        column: x => x.EmsProfileId,
                        principalTable: "EmsProfiles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_EmsSchedules_PIBTemplateLocations_LocationId",
                        column: x => x.LocationId,
                        principalTable: "PIBTemplateLocations",
                        principalColumn: "location_id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_EmsSchedules_User_UpdatedBy",
                        column: x => x.UpdatedBy,
                        principalTable: "User",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_EmsProfiles_CreatedBy",
                table: "EmsProfiles",
                column: "CreatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_EmsProfiles_UpdatedBy",
                table: "EmsProfiles",
                column: "UpdatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_EmsSchedules_CreatedBy",
                table: "EmsSchedules",
                column: "CreatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_EmsSchedules_EmsGroupId",
                table: "EmsSchedules",
                column: "EmsGroupId");

            migrationBuilder.CreateIndex(
                name: "IX_EmsSchedules_EmsProfileId",
                table: "EmsSchedules",
                column: "EmsProfileId");

            migrationBuilder.CreateIndex(
                name: "IX_EmsSchedules_LocationId",
                table: "EmsSchedules",
                column: "LocationId");

            migrationBuilder.CreateIndex(
                name: "IX_EmsSchedules_UpdatedBy",
                table: "EmsSchedules",
                column: "UpdatedBy");

            migrationBuilder.AddForeignKey(
                name: "FK_Devices_Emses_ems_id",
                table: "Devices",
                column: "ems_id",
                principalTable: "Emses",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Emses_User_CreatedBy",
                table: "Emses",
                column: "CreatedBy",
                principalTable: "User",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Emses_Institutions_InstitutionId",
                table: "Emses",
                column: "InstitutionId",
                principalTable: "Institutions",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Emses_User_UpdatedBy",
                table: "Emses",
                column: "UpdatedBy",
                principalTable: "User",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Devices_Emses_ems_id",
                table: "Devices");

            migrationBuilder.DropForeignKey(
                name: "FK_Emses_User_CreatedBy",
                table: "Emses");

            migrationBuilder.DropForeignKey(
                name: "FK_Emses_Institutions_InstitutionId",
                table: "Emses");

            migrationBuilder.DropForeignKey(
                name: "FK_Emses_User_UpdatedBy",
                table: "Emses");

            migrationBuilder.DropTable(
                name: "EmsSchedules");

            migrationBuilder.DropTable(
                name: "EmsProfiles");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Emses",
                table: "Emses");

            migrationBuilder.RenameTable(
                name: "Emses",
                newName: "Ems");

            migrationBuilder.RenameIndex(
                name: "IX_Emses_UpdatedBy",
                table: "Ems",
                newName: "IX_Ems_UpdatedBy");

            migrationBuilder.RenameIndex(
                name: "IX_Emses_InstitutionId",
                table: "Ems",
                newName: "IX_Ems_InstitutionId");

            migrationBuilder.RenameIndex(
                name: "IX_Emses_CreatedBy",
                table: "Ems",
                newName: "IX_Ems_CreatedBy");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Ems",
                table: "Ems",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Devices_Ems_ems_id",
                table: "Devices",
                column: "ems_id",
                principalTable: "Ems",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Ems_User_CreatedBy",
                table: "Ems",
                column: "CreatedBy",
                principalTable: "User",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Ems_Institutions_InstitutionId",
                table: "Ems",
                column: "InstitutionId",
                principalTable: "Institutions",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Ems_User_UpdatedBy",
                table: "Ems",
                column: "UpdatedBy",
                principalTable: "User",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
