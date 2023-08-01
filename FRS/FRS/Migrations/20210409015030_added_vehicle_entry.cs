using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace FRS.Migrations
{
    public partial class added_vehicle_entry : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "VehicleEntries",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    IsActive = table.Column<bool>(nullable: false),
                    CreatedBy = table.Column<int>(nullable: true),
                    UpdatedBy = table.Column<int>(nullable: true),
                    UpdatedDate = table.Column<DateTime>(nullable: false),
                    CreatedDate = table.Column<DateTime>(nullable: false),
                    SeasonId = table.Column<string>(nullable: true),
                    PersonName = table.Column<string>(nullable: true),
                    CardType = table.Column<string>(nullable: true),
                    Vehicle = table.Column<string>(nullable: true),
                    VehicleStatus = table.Column<string>(nullable: true),
                    IssueDate = table.Column<DateTime>(nullable: true),
                    ExpiryDate = table.Column<DateTime>(nullable: true),
                    Type = table.Column<string>(nullable: true),
                    Timestamp = table.Column<DateTime>(nullable: true),
                    ReservationId = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VehicleEntries", x => x.Id);
                    table.ForeignKey(
                        name: "FK_VehicleEntries_User_CreatedBy",
                        column: x => x.CreatedBy,
                        principalTable: "User",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_VehicleEntries_Reservations_ReservationId",
                        column: x => x.ReservationId,
                        principalTable: "Reservations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_VehicleEntries_User_UpdatedBy",
                        column: x => x.UpdatedBy,
                        principalTable: "User",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_VehicleEntries_CreatedBy",
                table: "VehicleEntries",
                column: "CreatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_VehicleEntries_ReservationId",
                table: "VehicleEntries",
                column: "ReservationId");

            migrationBuilder.CreateIndex(
                name: "IX_VehicleEntries_UpdatedBy",
                table: "VehicleEntries",
                column: "UpdatedBy");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "VehicleEntries");
        }
    }
}
