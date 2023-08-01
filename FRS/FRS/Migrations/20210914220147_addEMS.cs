using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace FRS.Migrations
{
    public partial class addEMS : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Ems",
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
                    Description = table.Column<string>(nullable: true),
                    InstitutionId = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Ems", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Ems_User_CreatedBy",
                        column: x => x.CreatedBy,
                        principalTable: "User",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Ems_Institutions_InstitutionId",
                        column: x => x.InstitutionId,
                        principalTable: "Institutions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Ems_User_UpdatedBy",
                        column: x => x.UpdatedBy,
                        principalTable: "User",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Devices_ems_id",
                table: "Devices",
                column: "ems_id");

            migrationBuilder.CreateIndex(
                name: "IX_Ems_CreatedBy",
                table: "Ems",
                column: "CreatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_Ems_InstitutionId",
                table: "Ems",
                column: "InstitutionId");

            migrationBuilder.CreateIndex(
                name: "IX_Ems_UpdatedBy",
                table: "Ems",
                column: "UpdatedBy");

            migrationBuilder.AddForeignKey(
                name: "FK_Devices_Ems_ems_id",
                table: "Devices",
                column: "ems_id",
                principalTable: "Ems",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Devices_Ems_ems_id",
                table: "Devices");

            migrationBuilder.DropTable(
                name: "Ems");

            migrationBuilder.DropIndex(
                name: "IX_Devices_ems_id",
                table: "Devices");
        }
    }
}
