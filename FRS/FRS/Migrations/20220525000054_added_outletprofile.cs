using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace FRS.Migrations
{
    public partial class added_outletprofile : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "OutletProfileId",
                table: "Outlets",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "CuisineId",
                table: "Dishes",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Cuisines",
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
                    InstitutionId = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Cuisines", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Cuisines_User_CreatedBy",
                        column: x => x.CreatedBy,
                        principalTable: "User",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Cuisines_Institutions_InstitutionId",
                        column: x => x.InstitutionId,
                        principalTable: "Institutions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Cuisines_User_UpdatedBy",
                        column: x => x.UpdatedBy,
                        principalTable: "User",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "OutletProfiles",
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
                    Address = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OutletProfiles", x => x.Id);
                    table.ForeignKey(
                        name: "FK_OutletProfiles_User_CreatedBy",
                        column: x => x.CreatedBy,
                        principalTable: "User",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_OutletProfiles_User_UpdatedBy",
                        column: x => x.UpdatedBy,
                        principalTable: "User",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Outlets_OutletProfileId",
                table: "Outlets",
                column: "OutletProfileId");

            migrationBuilder.CreateIndex(
                name: "IX_Dishes_CuisineId",
                table: "Dishes",
                column: "CuisineId");

            migrationBuilder.CreateIndex(
                name: "IX_Cuisines_CreatedBy",
                table: "Cuisines",
                column: "CreatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_Cuisines_InstitutionId",
                table: "Cuisines",
                column: "InstitutionId");

            migrationBuilder.CreateIndex(
                name: "IX_Cuisines_UpdatedBy",
                table: "Cuisines",
                column: "UpdatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_OutletProfiles_CreatedBy",
                table: "OutletProfiles",
                column: "CreatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_OutletProfiles_UpdatedBy",
                table: "OutletProfiles",
                column: "UpdatedBy");

            migrationBuilder.AddForeignKey(
                name: "FK_Dishes_Cuisines_CuisineId",
                table: "Dishes",
                column: "CuisineId",
                principalTable: "Cuisines",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Outlets_OutletProfiles_OutletProfileId",
                table: "Outlets",
                column: "OutletProfileId",
                principalTable: "OutletProfiles",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Dishes_Cuisines_CuisineId",
                table: "Dishes");

            migrationBuilder.DropForeignKey(
                name: "FK_Outlets_OutletProfiles_OutletProfileId",
                table: "Outlets");

            migrationBuilder.DropTable(
                name: "Cuisines");

            migrationBuilder.DropTable(
                name: "OutletProfiles");

            migrationBuilder.DropIndex(
                name: "IX_Outlets_OutletProfileId",
                table: "Outlets");

            migrationBuilder.DropIndex(
                name: "IX_Dishes_CuisineId",
                table: "Dishes");

            migrationBuilder.DropColumn(
                name: "OutletProfileId",
                table: "Outlets");

            migrationBuilder.DropColumn(
                name: "CuisineId",
                table: "Dishes");
        }
    }
}
