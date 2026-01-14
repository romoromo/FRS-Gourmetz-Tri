using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace FRS.Migrations
{
    /// <inheritdoc />
    public partial class SQLAppLockForBackground : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "FasRunLog",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ClassLevelId = table.Column<int>(type: "int", nullable: false),
                    RunKey = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FasRunLog", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_FasRunLog_ClassLevelId_CreatedAt",
                table: "FasRunLog",
                columns: new[] { "ClassLevelId", "CreatedAt" });

            migrationBuilder.CreateIndex(
                name: "IX_FasRunLog_RunKey",
                table: "FasRunLog",
                column: "RunKey",
                unique: true,
                filter: "[RunKey] IS NOT NULL");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "FasRunLog");
        }
    }
}
