using Microsoft.EntityFrameworkCore.Migrations;

namespace FRS.Migrations
{
    /// <inheritdoc />
    public partial class AddClasslevelDetail : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ClassLevelDetail",
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
                    table.PrimaryKey("PK_ClassLevelDetail", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ClassLevelDetail_ClassLevels_ClassLevelId",
                        column: x => x.ClassLevelId,
                        principalTable: "ClassLevels",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ClassLevelDetail_MealSessionDetails_SessionId",
                        column: x => x.SessionId,
                        principalTable: "MealSessionDetails",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_ClassLevelDetail_MealSessions_PeriodId",
                        column: x => x.PeriodId,
                        principalTable: "MealSessions",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_ClassLevelDetail_ClassLevelId",
                table: "ClassLevelDetail",
                column: "ClassLevelId");

            migrationBuilder.CreateIndex(
                name: "IX_ClassLevelDetail_PeriodId",
                table: "ClassLevelDetail",
                column: "PeriodId");

            migrationBuilder.CreateIndex(
                name: "IX_ClassLevelDetail_SessionId",
                table: "ClassLevelDetail",
                column: "SessionId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ClassLevelDetail");
        }
    }
}
