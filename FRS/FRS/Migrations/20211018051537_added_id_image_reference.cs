using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace FRS.Migrations
{
    public partial class added_id_image_reference : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_LocationImageReferences_Files_FileId",
                table: "LocationImageReferences");

            migrationBuilder.DropPrimaryKey(
                name: "PK_LocationImageReferences",
                table: "LocationImageReferences");

            migrationBuilder.AlterColumn<int>(
                name: "FileId",
                table: "LocationImageReferences",
                nullable: true,
                oldClrType: typeof(int));

            migrationBuilder.AddColumn<int>(
                name: "Id",
                table: "LocationImageReferences",
                nullable: false,
                defaultValue: 0)
                .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            migrationBuilder.AddPrimaryKey(
                name: "PK_LocationImageReferences",
                table: "LocationImageReferences",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_LocationImageReferences_LocationId",
                table: "LocationImageReferences",
                column: "LocationId");

            migrationBuilder.AddForeignKey(
                name: "FK_LocationImageReferences_Files_FileId",
                table: "LocationImageReferences",
                column: "FileId",
                principalTable: "Files",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_LocationImageReferences_Files_FileId",
                table: "LocationImageReferences");

            migrationBuilder.DropPrimaryKey(
                name: "PK_LocationImageReferences",
                table: "LocationImageReferences");

            migrationBuilder.DropIndex(
                name: "IX_LocationImageReferences_LocationId",
                table: "LocationImageReferences");

            migrationBuilder.DropColumn(
                name: "Id",
                table: "LocationImageReferences");

            migrationBuilder.AlterColumn<int>(
                name: "FileId",
                table: "LocationImageReferences",
                nullable: false,
                oldClrType: typeof(int),
                oldNullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_LocationImageReferences",
                table: "LocationImageReferences",
                columns: new[] { "LocationId", "FileId" });

            migrationBuilder.AddForeignKey(
                name: "FK_LocationImageReferences_Files_FileId",
                table: "LocationImageReferences",
                column: "FileId",
                principalTable: "Files",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
