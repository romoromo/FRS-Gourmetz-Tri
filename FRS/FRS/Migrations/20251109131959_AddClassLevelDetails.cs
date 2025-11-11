using Microsoft.EntityFrameworkCore.Migrations;

namespace FRS.Migrations
{
    /// <inheritdoc />
    public partial class AddClassLevelDetails : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ClassLevelDetails",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ClassLevelId = table.Column<int>(type: "int", nullable: false),
                    PeriodId = table.Column<int>(type: "int", nullable: true),
                    SessionId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ClassLevelDetails", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ClassLevelDetails_ClassLevels_ClassLevelId",
                        column: x => x.ClassLevelId,
                        principalTable: "ClassLevels",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ClassLevelDetails_MealSessionDetails_SessionId",
                        column: x => x.SessionId,
                        principalTable: "MealSessionDetails",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_ClassLevelDetails_MealSessions_PeriodId",
                        column: x => x.PeriodId,
                        principalTable: "MealSessions",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_ClassLevelDetails_ClassLevelId",
                table: "ClassLevelDetails",
                column: "ClassLevelId");

            migrationBuilder.CreateIndex(
                name: "IX_ClassLevelDetails_PeriodId",
                table: "ClassLevelDetails",
                column: "PeriodId");

            migrationBuilder.CreateIndex(
                name: "IX_ClassLevelDetails_SessionId_PeriodId",
                table: "ClassLevelDetails",
                columns: new[] { "SessionId", "PeriodId" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ClassLevelDetails");
        }
    }
}
