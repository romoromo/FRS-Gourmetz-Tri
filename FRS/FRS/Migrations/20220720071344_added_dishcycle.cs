using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace FRS.Migrations
{
    public partial class added_dishcycle : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "DishCycles",
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
                    Sequence = table.Column<int>(nullable: false),
                    DishTypeId = table.Column<int>(nullable: false),
                    StartDate = table.Column<DateTime>(nullable: false),
                    EndDate = table.Column<DateTime>(nullable: true),
                    NumOfDays = table.Column<int>(nullable: false),
                    NumOfSets = table.Column<int>(nullable: false),
                    OutletProfileId = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DishCycles", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DishCycles_User_CreatedBy",
                        column: x => x.CreatedBy,
                        principalTable: "User",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_DishCycles_DishTypes_DishTypeId",
                        column: x => x.DishTypeId,
                        principalTable: "DishTypes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_DishCycles_OutletProfiles_OutletProfileId",
                        column: x => x.OutletProfileId,
                        principalTable: "OutletProfiles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_DishCycles_User_UpdatedBy",
                        column: x => x.UpdatedBy,
                        principalTable: "User",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "DishCycleSchedules",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    IsActive = table.Column<bool>(nullable: false),
                    CreatedBy = table.Column<int>(nullable: true),
                    UpdatedBy = table.Column<int>(nullable: true),
                    UpdatedDate = table.Column<DateTime>(nullable: false),
                    CreatedDate = table.Column<DateTime>(nullable: false),
                    DishCycleId = table.Column<int>(nullable: false),
                    Day = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DishCycleSchedules", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DishCycleSchedules_User_CreatedBy",
                        column: x => x.CreatedBy,
                        principalTable: "User",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_DishCycleSchedules_DishCycles_DishCycleId",
                        column: x => x.DishCycleId,
                        principalTable: "DishCycles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_DishCycleSchedules_User_UpdatedBy",
                        column: x => x.UpdatedBy,
                        principalTable: "User",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "DishCycleScheduleDetails",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    IsActive = table.Column<bool>(nullable: false),
                    CreatedBy = table.Column<int>(nullable: true),
                    UpdatedBy = table.Column<int>(nullable: true),
                    UpdatedDate = table.Column<DateTime>(nullable: false),
                    CreatedDate = table.Column<DateTime>(nullable: false),
                    DishCycleScheduleId = table.Column<int>(nullable: false),
                    Label = table.Column<string>(nullable: true),
                    Sequence = table.Column<int>(nullable: false),
                    DishId = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DishCycleScheduleDetails", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DishCycleScheduleDetails_User_CreatedBy",
                        column: x => x.CreatedBy,
                        principalTable: "User",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_DishCycleScheduleDetails_DishCycleSchedules_DishCycleScheduleId",
                        column: x => x.DishCycleScheduleId,
                        principalTable: "DishCycleSchedules",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_DishCycleScheduleDetails_Dishes_DishId",
                        column: x => x.DishId,
                        principalTable: "Dishes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_DishCycleScheduleDetails_User_UpdatedBy",
                        column: x => x.UpdatedBy,
                        principalTable: "User",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_DishCycles_CreatedBy",
                table: "DishCycles",
                column: "CreatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_DishCycles_DishTypeId",
                table: "DishCycles",
                column: "DishTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_DishCycles_OutletProfileId",
                table: "DishCycles",
                column: "OutletProfileId");

            migrationBuilder.CreateIndex(
                name: "IX_DishCycles_UpdatedBy",
                table: "DishCycles",
                column: "UpdatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_DishCycleScheduleDetails_CreatedBy",
                table: "DishCycleScheduleDetails",
                column: "CreatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_DishCycleScheduleDetails_DishCycleScheduleId",
                table: "DishCycleScheduleDetails",
                column: "DishCycleScheduleId");

            migrationBuilder.CreateIndex(
                name: "IX_DishCycleScheduleDetails_DishId",
                table: "DishCycleScheduleDetails",
                column: "DishId");

            migrationBuilder.CreateIndex(
                name: "IX_DishCycleScheduleDetails_UpdatedBy",
                table: "DishCycleScheduleDetails",
                column: "UpdatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_DishCycleSchedules_CreatedBy",
                table: "DishCycleSchedules",
                column: "CreatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_DishCycleSchedules_DishCycleId",
                table: "DishCycleSchedules",
                column: "DishCycleId");

            migrationBuilder.CreateIndex(
                name: "IX_DishCycleSchedules_UpdatedBy",
                table: "DishCycleSchedules",
                column: "UpdatedBy");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DishCycleScheduleDetails");

            migrationBuilder.DropTable(
                name: "DishCycleSchedules");

            migrationBuilder.DropTable(
                name: "DishCycles");
        }
    }
}
