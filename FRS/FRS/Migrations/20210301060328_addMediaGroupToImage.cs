using Microsoft.EntityFrameworkCore.Migrations;

namespace FRS.Migrations
{
    public partial class addMediaGroupToImage : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "animation",
                table: "PlaylistImages",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "duration",
                table: "PlaylistImages",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsVideo",
                table: "ImageFiles",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "MediaId",
                table: "ImageFiles",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_ImageFiles_MediaId",
                table: "ImageFiles",
                column: "MediaId");

            migrationBuilder.AddForeignKey(
                name: "FK_ImageFiles_Medias_MediaId",
                table: "ImageFiles",
                column: "MediaId",
                principalTable: "Medias",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ImageFiles_Medias_MediaId",
                table: "ImageFiles");

            migrationBuilder.DropIndex(
                name: "IX_ImageFiles_MediaId",
                table: "ImageFiles");

            migrationBuilder.DropColumn(
                name: "animation",
                table: "PlaylistImages");

            migrationBuilder.DropColumn(
                name: "duration",
                table: "PlaylistImages");

            migrationBuilder.DropColumn(
                name: "IsVideo",
                table: "ImageFiles");

            migrationBuilder.DropColumn(
                name: "MediaId",
                table: "ImageFiles");
        }
    }
}
