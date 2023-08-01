using Microsoft.EntityFrameworkCore.Migrations;

namespace FRS.Migrations
{
    public partial class fixplaylistImage : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PlaylistImages_ImageFiles_ImageFileId",
                table: "PlaylistImages");

            migrationBuilder.DropIndex(
                name: "IX_PlaylistImages_ImageFileId",
                table: "PlaylistImages");

            migrationBuilder.DropColumn(
                name: "ImageFileId",
                table: "PlaylistImages");

            migrationBuilder.CreateIndex(
                name: "IX_PlaylistImages_ImageId",
                table: "PlaylistImages",
                column: "ImageId");

            migrationBuilder.AddForeignKey(
                name: "FK_PlaylistImages_ImageFiles_ImageId",
                table: "PlaylistImages",
                column: "ImageId",
                principalTable: "ImageFiles",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PlaylistImages_ImageFiles_ImageId",
                table: "PlaylistImages");

            migrationBuilder.DropIndex(
                name: "IX_PlaylistImages_ImageId",
                table: "PlaylistImages");

            migrationBuilder.AddColumn<int>(
                name: "ImageFileId",
                table: "PlaylistImages",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_PlaylistImages_ImageFileId",
                table: "PlaylistImages",
                column: "ImageFileId");

            migrationBuilder.AddForeignKey(
                name: "FK_PlaylistImages_ImageFiles_ImageFileId",
                table: "PlaylistImages",
                column: "ImageFileId",
                principalTable: "ImageFiles",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
