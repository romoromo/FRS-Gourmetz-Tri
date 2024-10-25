using System;
using Microsoft.EntityFrameworkCore.Migrations;

//#nullable disable

namespace FRS.Migrations
{
    /// <inheritdoc />
    public partial class dispenser_outlet_2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "DispenserOutlets",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    DispenserCode = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CounterName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Password = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LocationCode = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Color = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PLCIPAddress = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PLCPort = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PLCToken = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PLCApiVer = table.Column<int>(type: "int", nullable: false),
                    InstitutionId = table.Column<int>(type: "int", nullable: true),
                    OutletId = table.Column<int>(type: "int", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    CreatedBy = table.Column<int>(type: "int", nullable: true),
                    UpdatedBy = table.Column<int>(type: "int", nullable: true),
                    UpdatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DispenserOutlets", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DispenserOutlets_Institutions_InstitutionId",
                        column: x => x.InstitutionId,
                        principalTable: "Institutions",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_DispenserOutlets_Outlets_OutletId",
                        column: x => x.OutletId,
                        principalTable: "Outlets",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_DispenserOutlets_User_CreatedBy",
                        column: x => x.CreatedBy,
                        principalTable: "User",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_DispenserOutlets_User_UpdatedBy",
                        column: x => x.UpdatedBy,
                        principalTable: "User",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_DispenserOutlets_CreatedBy",
                table: "DispenserOutlets",
                column: "CreatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_DispenserOutlets_InstitutionId",
                table: "DispenserOutlets",
                column: "InstitutionId");

            migrationBuilder.CreateIndex(
                name: "IX_DispenserOutlets_OutletId",
                table: "DispenserOutlets",
                column: "OutletId");

            migrationBuilder.CreateIndex(
                name: "IX_DispenserOutlets_UpdatedBy",
                table: "DispenserOutlets",
                column: "UpdatedBy");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DispenserOutlets");
        }
    }
}
