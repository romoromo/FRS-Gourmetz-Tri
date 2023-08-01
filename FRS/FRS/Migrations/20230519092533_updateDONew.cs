using Microsoft.EntityFrameworkCore.Migrations;

namespace FRS.Migrations
{
    public partial class updateDONew : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_DeliveryDetailNews_Dishes_DishId",
                table: "DeliveryDetailNews");

            migrationBuilder.DropIndex(
                name: "IX_DeliveryDetailNews_DishId",
                table: "DeliveryDetailNews");

            migrationBuilder.DropColumn(
                name: "DishId",
                table: "DeliveryDetailNews");

            migrationBuilder.DropColumn(
                name: "Qty",
                table: "DeliveryDetailNews");

            migrationBuilder.AddColumn<string>(
                name: "CartonAssetCode",
                table: "DeliveryDetailNews",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CartonType",
                table: "DeliveryDetailNews",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "BentoAssetCode",
                table: "DeliveryBentoNews",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "BentoType",
                table: "DeliveryBentoNews",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DishCode",
                table: "DeliveryBentoNews",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "DishId",
                table: "DeliveryBentoNews",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DishLabel",
                table: "DeliveryBentoNews",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DishType",
                table: "DeliveryBentoNews",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Qty",
                table: "DeliveryBentoNews",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_DeliveryBentoNews_DishId",
                table: "DeliveryBentoNews",
                column: "DishId");

            migrationBuilder.AddForeignKey(
                name: "FK_DeliveryBentoNews_Dishes_DishId",
                table: "DeliveryBentoNews",
                column: "DishId",
                principalTable: "Dishes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_DeliveryBentoNews_Dishes_DishId",
                table: "DeliveryBentoNews");

            migrationBuilder.DropIndex(
                name: "IX_DeliveryBentoNews_DishId",
                table: "DeliveryBentoNews");

            migrationBuilder.DropColumn(
                name: "CartonAssetCode",
                table: "DeliveryDetailNews");

            migrationBuilder.DropColumn(
                name: "CartonType",
                table: "DeliveryDetailNews");

            migrationBuilder.DropColumn(
                name: "BentoAssetCode",
                table: "DeliveryBentoNews");

            migrationBuilder.DropColumn(
                name: "BentoType",
                table: "DeliveryBentoNews");

            migrationBuilder.DropColumn(
                name: "DishCode",
                table: "DeliveryBentoNews");

            migrationBuilder.DropColumn(
                name: "DishId",
                table: "DeliveryBentoNews");

            migrationBuilder.DropColumn(
                name: "DishLabel",
                table: "DeliveryBentoNews");

            migrationBuilder.DropColumn(
                name: "DishType",
                table: "DeliveryBentoNews");

            migrationBuilder.DropColumn(
                name: "Qty",
                table: "DeliveryBentoNews");

            migrationBuilder.AddColumn<int>(
                name: "DishId",
                table: "DeliveryDetailNews",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Qty",
                table: "DeliveryDetailNews",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_DeliveryDetailNews_DishId",
                table: "DeliveryDetailNews",
                column: "DishId");

            migrationBuilder.AddForeignKey(
                name: "FK_DeliveryDetailNews_Dishes_DishId",
                table: "DeliveryDetailNews",
                column: "DishId",
                principalTable: "Dishes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
