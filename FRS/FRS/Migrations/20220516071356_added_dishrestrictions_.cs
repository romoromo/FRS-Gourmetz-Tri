using Microsoft.EntityFrameworkCore.Migrations;

namespace FRS.Migrations
{
    public partial class added_dishrestrictions_ : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_DishRestriction_User_CreatedBy",
                table: "DishRestriction");

            migrationBuilder.DropForeignKey(
                name: "FK_DishRestriction_Dishes_DishId",
                table: "DishRestriction");

            migrationBuilder.DropForeignKey(
                name: "FK_DishRestriction_Restrictions_RestrictionId",
                table: "DishRestriction");

            migrationBuilder.DropForeignKey(
                name: "FK_DishRestriction_User_UpdatedBy",
                table: "DishRestriction");

            migrationBuilder.DropPrimaryKey(
                name: "PK_DishRestriction",
                table: "DishRestriction");

            migrationBuilder.RenameTable(
                name: "DishRestriction",
                newName: "DishRestrictions");

            migrationBuilder.RenameIndex(
                name: "IX_DishRestriction_UpdatedBy",
                table: "DishRestrictions",
                newName: "IX_DishRestrictions_UpdatedBy");

            migrationBuilder.RenameIndex(
                name: "IX_DishRestriction_RestrictionId",
                table: "DishRestrictions",
                newName: "IX_DishRestrictions_RestrictionId");

            migrationBuilder.RenameIndex(
                name: "IX_DishRestriction_DishId",
                table: "DishRestrictions",
                newName: "IX_DishRestrictions_DishId");

            migrationBuilder.RenameIndex(
                name: "IX_DishRestriction_CreatedBy",
                table: "DishRestrictions",
                newName: "IX_DishRestrictions_CreatedBy");

            migrationBuilder.AddPrimaryKey(
                name: "PK_DishRestrictions",
                table: "DishRestrictions",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_DishRestrictions_User_CreatedBy",
                table: "DishRestrictions",
                column: "CreatedBy",
                principalTable: "User",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_DishRestrictions_Dishes_DishId",
                table: "DishRestrictions",
                column: "DishId",
                principalTable: "Dishes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_DishRestrictions_Restrictions_RestrictionId",
                table: "DishRestrictions",
                column: "RestrictionId",
                principalTable: "Restrictions",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_DishRestrictions_User_UpdatedBy",
                table: "DishRestrictions",
                column: "UpdatedBy",
                principalTable: "User",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_DishRestrictions_User_CreatedBy",
                table: "DishRestrictions");

            migrationBuilder.DropForeignKey(
                name: "FK_DishRestrictions_Dishes_DishId",
                table: "DishRestrictions");

            migrationBuilder.DropForeignKey(
                name: "FK_DishRestrictions_Restrictions_RestrictionId",
                table: "DishRestrictions");

            migrationBuilder.DropForeignKey(
                name: "FK_DishRestrictions_User_UpdatedBy",
                table: "DishRestrictions");

            migrationBuilder.DropPrimaryKey(
                name: "PK_DishRestrictions",
                table: "DishRestrictions");

            migrationBuilder.RenameTable(
                name: "DishRestrictions",
                newName: "DishRestriction");

            migrationBuilder.RenameIndex(
                name: "IX_DishRestrictions_UpdatedBy",
                table: "DishRestriction",
                newName: "IX_DishRestriction_UpdatedBy");

            migrationBuilder.RenameIndex(
                name: "IX_DishRestrictions_RestrictionId",
                table: "DishRestriction",
                newName: "IX_DishRestriction_RestrictionId");

            migrationBuilder.RenameIndex(
                name: "IX_DishRestrictions_DishId",
                table: "DishRestriction",
                newName: "IX_DishRestriction_DishId");

            migrationBuilder.RenameIndex(
                name: "IX_DishRestrictions_CreatedBy",
                table: "DishRestriction",
                newName: "IX_DishRestriction_CreatedBy");

            migrationBuilder.AddPrimaryKey(
                name: "PK_DishRestriction",
                table: "DishRestriction",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_DishRestriction_User_CreatedBy",
                table: "DishRestriction",
                column: "CreatedBy",
                principalTable: "User",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_DishRestriction_Dishes_DishId",
                table: "DishRestriction",
                column: "DishId",
                principalTable: "Dishes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_DishRestriction_Restrictions_RestrictionId",
                table: "DishRestriction",
                column: "RestrictionId",
                principalTable: "Restrictions",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_DishRestriction_User_UpdatedBy",
                table: "DishRestriction",
                column: "UpdatedBy",
                principalTable: "User",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
