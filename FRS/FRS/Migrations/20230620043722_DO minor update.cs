using Microsoft.EntityFrameworkCore.Migrations;

namespace FRS.Migrations
{
    public partial class DOminorupdate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_DeliveryOrderNews_MealSessionDetails_MealSessionId",
                table: "DeliveryOrderNews");

            migrationBuilder.DropIndex(
                name: "IX_DeliveryOrderNews_MealSessionId",
                table: "DeliveryOrderNews");

            migrationBuilder.DropColumn(
                name: "MealSessionId",
                table: "DeliveryOrderNews");

            migrationBuilder.CreateIndex(
                name: "IX_DeliveryOrderNews_MealSessionDetailId",
                table: "DeliveryOrderNews",
                column: "MealSessionDetailId");

            migrationBuilder.AddForeignKey(
                name: "FK_DeliveryOrderNews_MealSessionDetails_MealSessionDetailId",
                table: "DeliveryOrderNews",
                column: "MealSessionDetailId",
                principalTable: "MealSessionDetails",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_DeliveryOrderNews_MealSessionDetails_MealSessionDetailId",
                table: "DeliveryOrderNews");

            migrationBuilder.DropIndex(
                name: "IX_DeliveryOrderNews_MealSessionDetailId",
                table: "DeliveryOrderNews");

            migrationBuilder.AddColumn<int>(
                name: "MealSessionId",
                table: "DeliveryOrderNews",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_DeliveryOrderNews_MealSessionId",
                table: "DeliveryOrderNews",
                column: "MealSessionId");

            migrationBuilder.AddForeignKey(
                name: "FK_DeliveryOrderNews_MealSessionDetails_MealSessionId",
                table: "DeliveryOrderNews",
                column: "MealSessionId",
                principalTable: "MealSessionDetails",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
