using Microsoft.EntityFrameworkCore.Migrations;

namespace FRS.Migrations
{
    public partial class StoreInventNew : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {

            migrationBuilder.AlterColumn<int>(
                name: "DeliveryOrderID",
                table: "StoreInventories",
                nullable: true,
                oldClrType: typeof(int));

            migrationBuilder.AddColumn<int>(
                name: "DeliveryOrderNewID",
                table: "StoreInventories",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_StoreInventories_DeliveryOrderNewID",
                table: "StoreInventories",
                column: "DeliveryOrderNewID");

            migrationBuilder.AddForeignKey(
                name: "FK_StoreInventories_DeliveryOrderNews_DeliveryOrderNewID",
                table: "StoreInventories",
                column: "DeliveryOrderNewID",
                principalTable: "DeliveryOrderNews",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {

            migrationBuilder.DropForeignKey(
                name: "FK_StoreInventories_DeliveryOrderNews_DeliveryOrderNewID",
                table: "StoreInventories");

            migrationBuilder.DropIndex(
                name: "IX_StoreInventories_DeliveryOrderNewID",
                table: "StoreInventories");

            migrationBuilder.DropColumn(
                name: "DeliveryOrderNewID",
                table: "StoreInventories");

            migrationBuilder.AlterColumn<int>(
                name: "DeliveryOrderID",
                table: "StoreInventories",
                nullable: false,
                oldClrType: typeof(int),
                oldNullable: true);
        }
    }
}
