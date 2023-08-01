using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace FRS.Migrations
{
    public partial class addEmployeeScheduleSlot : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "EmployeeScheduleSlot",
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
                    ScheduleId = table.Column<int>(nullable: true),
                    EmployeeDataId = table.Column<int>(nullable: true),
                    LocationId = table.Column<int>(nullable: true),
                    ShiftId = table.Column<int>(nullable: true),
                    day = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EmployeeScheduleSlot", x => x.Id);
                    table.ForeignKey(
                        name: "FK_EmployeeScheduleSlot_User_CreatedBy",
                        column: x => x.CreatedBy,
                        principalTable: "User",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_EmployeeScheduleSlot_EmployeeData_EmployeeDataId",
                        column: x => x.EmployeeDataId,
                        principalTable: "EmployeeData",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_EmployeeScheduleSlot_Locations_LocationId",
                        column: x => x.LocationId,
                        principalTable: "Locations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_EmployeeScheduleSlot_EmployeeSchedule_ScheduleId",
                        column: x => x.ScheduleId,
                        principalTable: "EmployeeSchedule",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_EmployeeScheduleSlot_EmployeeScheduleShift_ShiftId",
                        column: x => x.ShiftId,
                        principalTable: "EmployeeScheduleShift",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_EmployeeScheduleSlot_User_UpdatedBy",
                        column: x => x.UpdatedBy,
                        principalTable: "User",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_EmployeeScheduleSlot_CreatedBy",
                table: "EmployeeScheduleSlot",
                column: "CreatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_EmployeeScheduleSlot_EmployeeDataId",
                table: "EmployeeScheduleSlot",
                column: "EmployeeDataId");

            migrationBuilder.CreateIndex(
                name: "IX_EmployeeScheduleSlot_LocationId",
                table: "EmployeeScheduleSlot",
                column: "LocationId");

            migrationBuilder.CreateIndex(
                name: "IX_EmployeeScheduleSlot_ScheduleId",
                table: "EmployeeScheduleSlot",
                column: "ScheduleId");

            migrationBuilder.CreateIndex(
                name: "IX_EmployeeScheduleSlot_ShiftId",
                table: "EmployeeScheduleSlot",
                column: "ShiftId");

            migrationBuilder.CreateIndex(
                name: "IX_EmployeeScheduleSlot_UpdatedBy",
                table: "EmployeeScheduleSlot",
                column: "UpdatedBy");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "EmployeeScheduleSlot");
        }
    }
}
