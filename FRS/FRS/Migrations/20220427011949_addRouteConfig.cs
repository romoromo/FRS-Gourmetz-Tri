using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace FRS.Migrations
{
    public partial class addRouteConfig : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            //migrationBuilder.DropForeignKey(
            //    name: "FK_Facilities_DirectoryListing_Id",
            //    table: "Facilities");

            migrationBuilder.AddColumn<string>(
                name: "arrow_color",
                table: "KioskSettings",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "arrow_image",
                table: "KioskSettings",
                nullable: true);

            migrationBuilder.AddColumn<float>(
                name: "arrow_speed",
                table: "KioskSettings",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "line_color",
                table: "KioskSettings",
                nullable: true);

            //migrationBuilder.AlterColumn<int>(
            //    name: "Id",
            //    table: "Facilities",
            //    nullable: false,
            //    oldClrType: typeof(int))
            //    .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "arrow_color",
                table: "KioskSettings");

            migrationBuilder.DropColumn(
                name: "arrow_image",
                table: "KioskSettings");

            migrationBuilder.DropColumn(
                name: "arrow_speed",
                table: "KioskSettings");

            migrationBuilder.DropColumn(
                name: "line_color",
                table: "KioskSettings");

            //migrationBuilder.AlterColumn<int>(
            //    name: "Id",
            //    table: "Facilities",
            //    nullable: false,
            //    oldClrType: typeof(int))
            //    .OldAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            //migrationBuilder.AddForeignKey(
            //    name: "FK_Facilities_DirectoryListing_Id",
            //    table: "Facilities",
            //    column: "Id",
            //    principalTable: "DirectoryListing",
            //    principalColumn: "Id",
            //    onDelete: ReferentialAction.Cascade);
        }
    }
}
