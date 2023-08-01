using Microsoft.EntityFrameworkCore.Migrations;

namespace FRS.Migrations
{
    public partial class added_location_image_ref_colorcode : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Type",
                table: "LocationImageReferences",
                newName: "ColorCode");

            migrationBuilder.AddColumn<int>(
                name: "ImageReferenceTypeId",
                table: "LocationImageReferences",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_LocationImageReferences_ImageReferenceTypeId",
                table: "LocationImageReferences",
                column: "ImageReferenceTypeId");

            migrationBuilder.AddForeignKey(
                name: "FK_LocationImageReferences_ImageReferenceTypes_ImageReferenceTypeId",
                table: "LocationImageReferences",
                column: "ImageReferenceTypeId",
                principalTable: "ImageReferenceTypes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_LocationImageReferences_ImageReferenceTypes_ImageReferenceTypeId",
                table: "LocationImageReferences");

            migrationBuilder.DropIndex(
                name: "IX_LocationImageReferences_ImageReferenceTypeId",
                table: "LocationImageReferences");

            migrationBuilder.DropColumn(
                name: "ImageReferenceTypeId",
                table: "LocationImageReferences");

            migrationBuilder.RenameColumn(
                name: "ColorCode",
                table: "LocationImageReferences",
                newName: "Type");
        }
    }
}
