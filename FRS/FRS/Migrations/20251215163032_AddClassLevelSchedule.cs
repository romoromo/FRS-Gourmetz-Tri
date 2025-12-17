using Microsoft.EntityFrameworkCore.Migrations;

namespace FRS.Migrations
{
    /// <inheritdoc />
    public partial class AddClassLevelSchedule : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ClassLevelSchedules",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ClassLevelId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ClassLevelSchedules", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ClassLevelSchedules_ClassLevels_ClassLevelId",
                        column: x => x.ClassLevelId,
                        principalTable: "ClassLevels",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ClassLevelScheduleItems",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PeriodId = table.Column<int>(type: "int", nullable: true),
                    MealPeriodId = table.Column<int>(type: "int", nullable: true),
                    SessionId = table.Column<int>(type: "int", nullable: true),
                    MealSessionId = table.Column<int>(type: "int", nullable: true),
                    Day = table.Column<int>(type: "int", nullable: false),
                    ClassLevelScheduleId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ClassLevelScheduleItems", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ClassLevelScheduleItems_ClassLevelSchedules_ClassLevelScheduleId",
                        column: x => x.ClassLevelScheduleId,
                        principalTable: "ClassLevelSchedules",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_ClassLevelScheduleItems_MealSessionDetails_MealSessionId",
                        column: x => x.MealSessionId,
                        principalTable: "MealSessionDetails",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_ClassLevelScheduleItems_MealSessions_MealPeriodId",
                        column: x => x.MealPeriodId,
                        principalTable: "MealSessions",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_ClassLevelScheduleItems_ClassLevelScheduleId",
                table: "ClassLevelScheduleItems",
                column: "ClassLevelScheduleId");

            migrationBuilder.CreateIndex(
                name: "IX_ClassLevelScheduleItems_MealPeriodId",
                table: "ClassLevelScheduleItems",
                column: "MealPeriodId");

            migrationBuilder.CreateIndex(
                name: "IX_ClassLevelScheduleItems_MealSessionId",
                table: "ClassLevelScheduleItems",
                column: "MealSessionId");

            migrationBuilder.CreateIndex(
                name: "IX_ClassLevelScheduleItems_SessionId_PeriodId_Day",
                table: "ClassLevelScheduleItems",
                columns: new[] { "SessionId", "PeriodId", "Day" });

            migrationBuilder.CreateIndex(
                name: "IX_ClassLevelSchedules_ClassLevelId",
                table: "ClassLevelSchedules",
                column: "ClassLevelId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ClassLevelScheduleItems");

            migrationBuilder.DropTable(
                name: "ClassLevelSchedules");
        }
    }
}
