using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace FRS.Migrations
{
    public partial class added_classroster : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "OutletClassRosters",
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
                    StartDate = table.Column<DateTime>(nullable: false),
                    EndDate = table.Column<DateTime>(nullable: true),
                    OutletProfileId = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OutletClassRosters", x => x.Id);
                    table.ForeignKey(
                        name: "FK_OutletClassRosters_User_CreatedBy",
                        column: x => x.CreatedBy,
                        principalTable: "User",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_OutletClassRosters_OutletProfiles_OutletProfileId",
                        column: x => x.OutletProfileId,
                        principalTable: "OutletProfiles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_OutletClassRosters_User_UpdatedBy",
                        column: x => x.UpdatedBy,
                        principalTable: "User",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "OutletClassRosterSchedules",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    IsActive = table.Column<bool>(nullable: false),
                    CreatedBy = table.Column<int>(nullable: true),
                    UpdatedBy = table.Column<int>(nullable: true),
                    UpdatedDate = table.Column<DateTime>(nullable: false),
                    CreatedDate = table.Column<DateTime>(nullable: false),
                    OutletClassRosterId = table.Column<int>(nullable: false),
                    Day = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OutletClassRosterSchedules", x => x.Id);
                    table.ForeignKey(
                        name: "FK_OutletClassRosterSchedules_User_CreatedBy",
                        column: x => x.CreatedBy,
                        principalTable: "User",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_OutletClassRosterSchedules_OutletClassRosters_OutletClassRosterId",
                        column: x => x.OutletClassRosterId,
                        principalTable: "OutletClassRosters",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_OutletClassRosterSchedules_User_UpdatedBy",
                        column: x => x.UpdatedBy,
                        principalTable: "User",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "OutletClassRosterSchedulePeriods",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    IsActive = table.Column<bool>(nullable: false),
                    CreatedBy = table.Column<int>(nullable: true),
                    UpdatedBy = table.Column<int>(nullable: true),
                    UpdatedDate = table.Column<DateTime>(nullable: false),
                    CreatedDate = table.Column<DateTime>(nullable: false),
                    OutletClassRosterScheduleId = table.Column<int>(nullable: false),
                    MealPeriodId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OutletClassRosterSchedulePeriods", x => x.Id);
                    table.ForeignKey(
                        name: "FK_OutletClassRosterSchedulePeriods_User_CreatedBy",
                        column: x => x.CreatedBy,
                        principalTable: "User",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_OutletClassRosterSchedulePeriods_MealPeriods_MealPeriodId",
                        column: x => x.MealPeriodId,
                        principalTable: "MealPeriods",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_OutletClassRosterSchedulePeriods_OutletClassRosterSchedules_OutletClassRosterScheduleId",
                        column: x => x.OutletClassRosterScheduleId,
                        principalTable: "OutletClassRosterSchedules",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_OutletClassRosterSchedulePeriods_User_UpdatedBy",
                        column: x => x.UpdatedBy,
                        principalTable: "User",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "OutletClassRosterSchedulePeriodClasses",
                columns: table => new
                {
                    OutletClassRosterSchedulePeriodId = table.Column<int>(nullable: false),
                    ClassId = table.Column<int>(nullable: false),
                    IsActive = table.Column<bool>(nullable: false),
                    CreatedBy = table.Column<int>(nullable: true),
                    UpdatedBy = table.Column<int>(nullable: true),
                    UpdatedDate = table.Column<DateTime>(nullable: false),
                    CreatedDate = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OutletClassRosterSchedulePeriodClasses", x => new { x.OutletClassRosterSchedulePeriodId, x.ClassId });
                    table.ForeignKey(
                        name: "FK_OutletClassRosterSchedulePeriodClasses_Classes_ClassId",
                        column: x => x.ClassId,
                        principalTable: "Classes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_OutletClassRosterSchedulePeriodClasses_User_CreatedBy",
                        column: x => x.CreatedBy,
                        principalTable: "User",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_OutletClassRosterSchedulePeriodClasses_OutletClassRosterSchedulePeriods_OutletClassRosterSchedulePeriodId",
                        column: x => x.OutletClassRosterSchedulePeriodId,
                        principalTable: "OutletClassRosterSchedulePeriods",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_OutletClassRosterSchedulePeriodClasses_User_UpdatedBy",
                        column: x => x.UpdatedBy,
                        principalTable: "User",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_OutletClassRosters_CreatedBy",
                table: "OutletClassRosters",
                column: "CreatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_OutletClassRosters_OutletProfileId",
                table: "OutletClassRosters",
                column: "OutletProfileId");

            migrationBuilder.CreateIndex(
                name: "IX_OutletClassRosters_UpdatedBy",
                table: "OutletClassRosters",
                column: "UpdatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_OutletClassRosterSchedulePeriodClasses_ClassId",
                table: "OutletClassRosterSchedulePeriodClasses",
                column: "ClassId");

            migrationBuilder.CreateIndex(
                name: "IX_OutletClassRosterSchedulePeriodClasses_CreatedBy",
                table: "OutletClassRosterSchedulePeriodClasses",
                column: "CreatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_OutletClassRosterSchedulePeriodClasses_UpdatedBy",
                table: "OutletClassRosterSchedulePeriodClasses",
                column: "UpdatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_OutletClassRosterSchedulePeriods_CreatedBy",
                table: "OutletClassRosterSchedulePeriods",
                column: "CreatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_OutletClassRosterSchedulePeriods_MealPeriodId",
                table: "OutletClassRosterSchedulePeriods",
                column: "MealPeriodId");

            migrationBuilder.CreateIndex(
                name: "IX_OutletClassRosterSchedulePeriods_OutletClassRosterScheduleId",
                table: "OutletClassRosterSchedulePeriods",
                column: "OutletClassRosterScheduleId");

            migrationBuilder.CreateIndex(
                name: "IX_OutletClassRosterSchedulePeriods_UpdatedBy",
                table: "OutletClassRosterSchedulePeriods",
                column: "UpdatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_OutletClassRosterSchedules_CreatedBy",
                table: "OutletClassRosterSchedules",
                column: "CreatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_OutletClassRosterSchedules_OutletClassRosterId",
                table: "OutletClassRosterSchedules",
                column: "OutletClassRosterId");

            migrationBuilder.CreateIndex(
                name: "IX_OutletClassRosterSchedules_UpdatedBy",
                table: "OutletClassRosterSchedules",
                column: "UpdatedBy");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "OutletClassRosterSchedulePeriodClasses");

            migrationBuilder.DropTable(
                name: "OutletClassRosterSchedulePeriods");

            migrationBuilder.DropTable(
                name: "OutletClassRosterSchedules");

            migrationBuilder.DropTable(
                name: "OutletClassRosters");
        }
    }
}
