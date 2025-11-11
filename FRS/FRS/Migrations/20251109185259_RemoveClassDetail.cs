using Microsoft.EntityFrameworkCore.Migrations;

namespace FRS.Migrations
{
    /// <inheritdoc />
    public partial class RemoveClassDetail : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ClassDetails");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ClassDetails",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ClassId = table.Column<int>(type: "int", nullable: false),
                    PeriodId = table.Column<int>(type: "int", nullable: true),
                    SessionId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ClassDetails", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ClassDetails_Classes_ClassId",
                        column: x => x.ClassId,
                        principalTable: "Classes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ClassDetails_MealSessionDetails_SessionId",
                        column: x => x.SessionId,
                        principalTable: "MealSessionDetails",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_ClassDetails_MealSessions_PeriodId",
                        column: x => x.PeriodId,
                        principalTable: "MealSessions",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_ClassDetails_ClassId",
                table: "ClassDetails",
                column: "ClassId");

            migrationBuilder.CreateIndex(
                name: "IX_ClassDetails_PeriodId",
                table: "ClassDetails",
                column: "PeriodId");

            migrationBuilder.CreateIndex(
                name: "IX_ClassDetails_SessionId_PeriodId",
                table: "ClassDetails",
                columns: new[] { "SessionId", "PeriodId" });
        }
    }
}
