using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace FRS.Migrations
{
    public partial class added_mealsession : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "MealSessions",
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
                    InstitutionId = table.Column<int>(nullable: true),
                    Sequence = table.Column<int>(nullable: false),
                    StartDate = table.Column<DateTime>(nullable: false),
                    EndDate = table.Column<DateTime>(nullable: true),
                    MealPeriodId = table.Column<int>(nullable: true),
                    OutletId = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MealSessions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MealSessions_User_CreatedBy",
                        column: x => x.CreatedBy,
                        principalTable: "User",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_MealSessions_Institutions_InstitutionId",
                        column: x => x.InstitutionId,
                        principalTable: "Institutions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_MealSessions_MealPeriods_MealPeriodId",
                        column: x => x.MealPeriodId,
                        principalTable: "MealPeriods",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_MealSessions_Outlets_OutletId",
                        column: x => x.OutletId,
                        principalTable: "Outlets",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_MealSessions_User_UpdatedBy",
                        column: x => x.UpdatedBy,
                        principalTable: "User",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "MealSessionDetails",
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
                    InstitutionId = table.Column<int>(nullable: true),
                    Sequence = table.Column<int>(nullable: false),
                    StartDate = table.Column<DateTime>(nullable: false),
                    EndDate = table.Column<DateTime>(nullable: true),
                    MealSessionId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MealSessionDetails", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MealSessionDetails_User_CreatedBy",
                        column: x => x.CreatedBy,
                        principalTable: "User",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_MealSessionDetails_MealSessions_MealSessionId",
                        column: x => x.MealSessionId,
                        principalTable: "MealSessions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_MealSessionDetails_User_UpdatedBy",
                        column: x => x.UpdatedBy,
                        principalTable: "User",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_MealSessionDetails_CreatedBy",
                table: "MealSessionDetails",
                column: "CreatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_MealSessionDetails_MealSessionId",
                table: "MealSessionDetails",
                column: "MealSessionId");

            migrationBuilder.CreateIndex(
                name: "IX_MealSessionDetails_UpdatedBy",
                table: "MealSessionDetails",
                column: "UpdatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_MealSessions_CreatedBy",
                table: "MealSessions",
                column: "CreatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_MealSessions_InstitutionId",
                table: "MealSessions",
                column: "InstitutionId");

            migrationBuilder.CreateIndex(
                name: "IX_MealSessions_MealPeriodId",
                table: "MealSessions",
                column: "MealPeriodId");

            migrationBuilder.CreateIndex(
                name: "IX_MealSessions_OutletId",
                table: "MealSessions",
                column: "OutletId");

            migrationBuilder.CreateIndex(
                name: "IX_MealSessions_UpdatedBy",
                table: "MealSessions",
                column: "UpdatedBy");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "MealSessionDetails");

            migrationBuilder.DropTable(
                name: "MealSessions");
        }
    }
}
