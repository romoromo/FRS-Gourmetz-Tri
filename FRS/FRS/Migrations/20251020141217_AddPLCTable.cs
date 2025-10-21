using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace FRS.Migrations
{
    /// <inheritdoc />
    public partial class AddPLCTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "PLCs",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    IPAddress = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    Framework = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    TotalNumber = table.Column<int>(type: "int", nullable: false),
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
                    table.PrimaryKey("PK_PLCs", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PLCs_Institutions_InstitutionId",
                        column: x => x.InstitutionId,
                        principalTable: "Institutions",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_PLCs_Outlets_OutletId",
                        column: x => x.OutletId,
                        principalTable: "Outlets",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_PLCs_User_CreatedBy",
                        column: x => x.CreatedBy,
                        principalTable: "User",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_PLCs_User_UpdatedBy",
                        column: x => x.UpdatedBy,
                        principalTable: "User",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_PLCs_CreatedBy",
                table: "PLCs",
                column: "CreatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_PLCs_InstitutionId",
                table: "PLCs",
                column: "InstitutionId");

            migrationBuilder.CreateIndex(
                name: "IX_PLCs_IPAddress_Framework_TotalNumber_OutletId_InstitutionId",
                table: "PLCs",
                columns: new[] { "IPAddress", "Framework", "TotalNumber", "OutletId", "InstitutionId" });

            migrationBuilder.CreateIndex(
                name: "IX_PLCs_OutletId",
                table: "PLCs",
                column: "OutletId");

            migrationBuilder.CreateIndex(
                name: "IX_PLCs_UpdatedBy",
                table: "PLCs",
                column: "UpdatedBy");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PLCs");
        }
    }
}
