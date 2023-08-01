using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace FRS.Migrations
{
    public partial class addEmployeeScheduleInfo : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "CoveringEmployeeDataId",
                table: "EmployeeScheduleSlot",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "EmployeeScheduleInfo",
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
                    LocationId = table.Column<int>(nullable: true),
                    ShiftId = table.Column<int>(nullable: true),
                    day = table.Column<int>(nullable: true),
                    extend = table.Column<bool>(nullable: false),
                    noDisplay = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EmployeeScheduleInfo", x => x.Id);
                    table.ForeignKey(
                        name: "FK_EmployeeScheduleInfo_User_CreatedBy",
                        column: x => x.CreatedBy,
                        principalTable: "User",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_EmployeeScheduleInfo_Locations_LocationId",
                        column: x => x.LocationId,
                        principalTable: "Locations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_EmployeeScheduleInfo_EmployeeSchedule_ScheduleId",
                        column: x => x.ScheduleId,
                        principalTable: "EmployeeSchedule",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_EmployeeScheduleInfo_EmployeeScheduleShift_ShiftId",
                        column: x => x.ShiftId,
                        principalTable: "EmployeeScheduleShift",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_EmployeeScheduleInfo_User_UpdatedBy",
                        column: x => x.UpdatedBy,
                        principalTable: "User",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_EmployeeScheduleSlot_CoveringEmployeeDataId",
                table: "EmployeeScheduleSlot",
                column: "CoveringEmployeeDataId");

            migrationBuilder.CreateIndex(
                name: "IX_EmployeeScheduleInfo_CreatedBy",
                table: "EmployeeScheduleInfo",
                column: "CreatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_EmployeeScheduleInfo_LocationId",
                table: "EmployeeScheduleInfo",
                column: "LocationId");

            migrationBuilder.CreateIndex(
                name: "IX_EmployeeScheduleInfo_ScheduleId",
                table: "EmployeeScheduleInfo",
                column: "ScheduleId");

            migrationBuilder.CreateIndex(
                name: "IX_EmployeeScheduleInfo_ShiftId",
                table: "EmployeeScheduleInfo",
                column: "ShiftId");

            migrationBuilder.CreateIndex(
                name: "IX_EmployeeScheduleInfo_UpdatedBy",
                table: "EmployeeScheduleInfo",
                column: "UpdatedBy");

            migrationBuilder.AddForeignKey(
                name: "FK_EmployeeScheduleSlot_EmployeeData_CoveringEmployeeDataId",
                table: "EmployeeScheduleSlot",
                column: "CoveringEmployeeDataId",
                principalTable: "EmployeeData",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_EmployeeScheduleSlot_EmployeeData_CoveringEmployeeDataId",
                table: "EmployeeScheduleSlot");

            migrationBuilder.DropTable(
                name: "EmployeeScheduleInfo");

            migrationBuilder.DropIndex(
                name: "IX_EmployeeScheduleSlot_CoveringEmployeeDataId",
                table: "EmployeeScheduleSlot");

            migrationBuilder.DropColumn(
                name: "CoveringEmployeeDataId",
                table: "EmployeeScheduleSlot");
        }
    }
}
