using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace FRS.Migrations
{
    /// <inheritdoc />
    public partial class AddTraysTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Trays",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    DispenserId = table.Column<int>(type: "int", nullable: false),
                    PLCId = table.Column<int>(type: "int", nullable: false),
                    MotorOutputNumber = table.Column<int>(type: "int", nullable: false),
                    LEDOutputNumber = table.Column<int>(type: "int", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    CreatedBy = table.Column<int>(type: "int", nullable: true),
                    UpdatedBy = table.Column<int>(type: "int", nullable: true),
                    UpdatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Trays", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Trays_DispenserOutlets_DispenserId",
                        column: x => x.DispenserId,
                        principalTable: "DispenserOutlets",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Trays_PLCs_PLCId",
                        column: x => x.PLCId,
                        principalTable: "PLCs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Trays_User_CreatedBy",
                        column: x => x.CreatedBy,
                        principalTable: "User",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Trays_User_UpdatedBy",
                        column: x => x.UpdatedBy,
                        principalTable: "User",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_Trays_CreatedBy",
                table: "Trays",
                column: "CreatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_Trays_DispenserId",
                table: "Trays",
                column: "DispenserId");

            migrationBuilder.CreateIndex(
                name: "IX_Trays_PLCId",
                table: "Trays",
                column: "PLCId");

            migrationBuilder.CreateIndex(
                name: "IX_Trays_UpdatedBy",
                table: "Trays",
                column: "UpdatedBy");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Trays");
        }
    }
}
