using Microsoft.EntityFrameworkCore.Migrations;


namespace FRS.Migrations
{
    /// <inheritdoc />
    public partial class AlterClassLevelDetail : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ClassLevelDetail_ClassLevels_ClassLevelId",
                table: "ClassLevelDetail");

            migrationBuilder.DropForeignKey(
                name: "FK_ClassLevelDetail_MealSessionDetails_SessionId",
                table: "ClassLevelDetail");

            migrationBuilder.DropForeignKey(
                name: "FK_ClassLevelDetail_MealSessions_PeriodId",
                table: "ClassLevelDetail");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ClassLevelDetail",
                table: "ClassLevelDetail");

            migrationBuilder.DropIndex(
                name: "IX_ClassLevelDetail_SessionId",
                table: "ClassLevelDetail");

            migrationBuilder.RenameTable(
                name: "ClassLevelDetail",
                newName: "ClassLevelDetails");

            migrationBuilder.RenameIndex(
                name: "IX_ClassLevelDetail_PeriodId",
                table: "ClassLevelDetails",
                newName: "IX_ClassLevelDetails_PeriodId");

            migrationBuilder.RenameIndex(
                name: "IX_ClassLevelDetail_ClassLevelId",
                table: "ClassLevelDetails",
                newName: "IX_ClassLevelDetails_ClassLevelId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ClassLevelDetails",
                table: "ClassLevelDetails",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_ClassLevelDetails_SessionId_PeriodId",
                table: "ClassLevelDetails",
                columns: new[] { "SessionId", "PeriodId" });

            migrationBuilder.AddForeignKey(
                name: "FK_ClassLevelDetails_ClassLevels_ClassLevelId",
                table: "ClassLevelDetails",
                column: "ClassLevelId",
                principalTable: "ClassLevels",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ClassLevelDetails_MealSessionDetails_SessionId",
                table: "ClassLevelDetails",
                column: "SessionId",
                principalTable: "MealSessionDetails",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_ClassLevelDetails_MealSessions_PeriodId",
                table: "ClassLevelDetails",
                column: "PeriodId",
                principalTable: "MealSessions",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ClassLevelDetails_ClassLevels_ClassLevelId",
                table: "ClassLevelDetails");

            migrationBuilder.DropForeignKey(
                name: "FK_ClassLevelDetails_MealSessionDetails_SessionId",
                table: "ClassLevelDetails");

            migrationBuilder.DropForeignKey(
                name: "FK_ClassLevelDetails_MealSessions_PeriodId",
                table: "ClassLevelDetails");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ClassLevelDetails",
                table: "ClassLevelDetails");

            migrationBuilder.DropIndex(
                name: "IX_ClassLevelDetails_SessionId_PeriodId",
                table: "ClassLevelDetails");

            migrationBuilder.RenameTable(
                name: "ClassLevelDetails",
                newName: "ClassLevelDetail");

            migrationBuilder.RenameIndex(
                name: "IX_ClassLevelDetails_PeriodId",
                table: "ClassLevelDetail",
                newName: "IX_ClassLevelDetail_PeriodId");

            migrationBuilder.RenameIndex(
                name: "IX_ClassLevelDetails_ClassLevelId",
                table: "ClassLevelDetail",
                newName: "IX_ClassLevelDetail_ClassLevelId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ClassLevelDetail",
                table: "ClassLevelDetail",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_ClassLevelDetail_SessionId",
                table: "ClassLevelDetail",
                column: "SessionId");

            migrationBuilder.AddForeignKey(
                name: "FK_ClassLevelDetail_ClassLevels_ClassLevelId",
                table: "ClassLevelDetail",
                column: "ClassLevelId",
                principalTable: "ClassLevels",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ClassLevelDetail_MealSessionDetails_SessionId",
                table: "ClassLevelDetail",
                column: "SessionId",
                principalTable: "MealSessionDetails",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_ClassLevelDetail_MealSessions_PeriodId",
                table: "ClassLevelDetail",
                column: "PeriodId",
                principalTable: "MealSessions",
                principalColumn: "Id");
        }
    }
}
