using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace FRS.Migrations
{
    public partial class changes_to_mealplan : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            //migrationBuilder.AddColumn<DateTime>(
            //    name: "Last2FAValidatedTime",
            //    table: "User",
            //    nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "AmendReason",
                table: "TokenOrders",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsMealPlan",
                table: "TokenOrders",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsStudentGroupOrder",
                table: "TokenOrders",
                nullable: false,
                defaultValue: false);

            //migrationBuilder.AddColumn<DateTime>(
            //    name: "DeliveryEndDate",
            //    table: "StudentGroups",
            //    nullable: true);

            //migrationBuilder.AddColumn<DateTime>(
            //    name: "DeliveryStartDate",
            //    table: "StudentGroups",
            //    nullable: true);

            //migrationBuilder.AddColumn<string>(
            //    name: "Description",
            //    table: "StudentGroups",
            //    nullable: true);

            //migrationBuilder.AddColumn<DateTime>(
            //    name: "EndDate",
            //    table: "StudentGroups",
            //    nullable: true);

            //migrationBuilder.AddColumn<string>(
            //    name: "FileName",
            //    table: "StudentGroups",
            //    nullable: true);

            //migrationBuilder.AddColumn<string>(
            //    name: "FilePath",
            //    table: "StudentGroups",
            //    nullable: true);

            //migrationBuilder.AddColumn<bool>(
            //    name: "IsPublished",
            //    table: "StudentGroups",
            //    nullable: false,
            //    defaultValue: false);

            //migrationBuilder.AddColumn<int>(
            //    name: "OutletTermId",
            //    table: "StudentGroups",
            //    nullable: true);

            //migrationBuilder.AddColumn<float>(
            //    name: "Price",
            //    table: "StudentGroups",
            //    nullable: false,
            //    defaultValue: 0f);

            //migrationBuilder.AddColumn<int>(
            //    name: "Sequence",
            //    table: "StudentGroups",
            //    nullable: false,
            //    defaultValue: 0);

            //migrationBuilder.AddColumn<DateTime>(
            //    name: "StartDate",
            //    table: "StudentGroups",
            //    nullable: true);

            //migrationBuilder.AddColumn<int>(
            //    name: "Term",
            //    table: "StudentGroups",
            //    nullable: false,
            //    defaultValue: 0);

            //migrationBuilder.AddColumn<string>(
            //    name: "Type",
            //    table: "StudentGroups",
            //    nullable: true);

            //migrationBuilder.AddColumn<string>(StudentGroups_
            //    name: "StudentGroupName",
            //    table: "spSalesOrderReport",
            //    nullable: true);

            //migrationBuilder.AddColumn<string>(
            //    name: "ActionName",
            //    table: "AuditLogs",
            //    nullable: true);

            //migrationBuilder.AddColumn<string>(
            //    name: "GroupId",
            //    table: "AuditLogs",
            //    nullable: true);

            //migrationBuilder.AddColumn<string>(
            //    name: "Remarks",
            //    table: "AuditLogs",
            //    nullable: true);

            migrationBuilder.CreateTable(
                name: "MealPlanOrders",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    IsActive = table.Column<bool>(nullable: false),
                    CreatedBy = table.Column<int>(nullable: true),
                    UpdatedBy = table.Column<int>(nullable: true),
                    UpdatedDate = table.Column<DateTime>(nullable: false),
                    CreatedDate = table.Column<DateTime>(nullable: false),
                    TransactionTime = table.Column<DateTime>(nullable: false),
                    ProfileId = table.Column<int>(nullable: true),
                    TotalAmount = table.Column<decimal>(nullable: false),
                    StoreId = table.Column<int>(nullable: true),
                    Status = table.Column<string>(nullable: true),
                    StudentGroupId = table.Column<int>(nullable: true),
                    PaymentId = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MealPlanOrders", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MealPlanOrders_User_CreatedBy",
                        column: x => x.CreatedBy,
                        principalTable: "User",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_MealPlanOrders_Payments_PaymentId",
                        column: x => x.PaymentId,
                        principalTable: "Payments",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_MealPlanOrders_Students_ProfileId",
                        column: x => x.ProfileId,
                        principalTable: "Students",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_MealPlanOrders_StoreInfos_StoreId",
                        column: x => x.StoreId,
                        principalTable: "StoreInfos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_MealPlanOrders_StudentGroups_StudentGroupId",
                        column: x => x.StudentGroupId,
                        principalTable: "StudentGroups",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_MealPlanOrders_User_UpdatedBy",
                        column: x => x.UpdatedBy,
                        principalTable: "User",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            //migrationBuilder.CreateTable(
            //    name: "OrderPortalContents",
            //    columns: table => new
            //    {
            //        Id = table.Column<int>(nullable: false)
            //            .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
            //        IsActive = table.Column<bool>(nullable: false),
            //        CreatedBy = table.Column<int>(nullable: true),
            //        UpdatedBy = table.Column<int>(nullable: true),
            //        UpdatedDate = table.Column<DateTime>(nullable: false),
            //        CreatedDate = table.Column<DateTime>(nullable: false),
            //        Announcement = table.Column<string>(nullable: true),
            //        Description = table.Column<string>(nullable: true),
            //        EffectiveStartDate = table.Column<DateTime>(nullable: false),
            //        EffectiveEndDate = table.Column<DateTime>(nullable: false),
            //        OutletId = table.Column<int>(nullable: true)
            //    },
            //    constraints: table =>
            //    {
            //        table.PrimaryKey("PK_OrderPortalContents", x => x.Id);
            //        table.ForeignKey(
            //            name: "FK_OrderPortalContents_User_CreatedBy",
            //            column: x => x.CreatedBy,
            //            principalTable: "User",
            //            principalColumn: "Id",
            //            onDelete: ReferentialAction.Restrict);
            //        table.ForeignKey(
            //            name: "FK_OrderPortalContents_Outlets_OutletId",
            //            column: x => x.OutletId,
            //            principalTable: "Outlets",
            //            principalColumn: "Id",
            //            onDelete: ReferentialAction.Restrict);
            //        table.ForeignKey(
            //            name: "FK_OrderPortalContents_User_UpdatedBy",
            //            column: x => x.UpdatedBy,
            //            principalTable: "User",
            //            principalColumn: "Id",
            //            onDelete: ReferentialAction.Restrict);
            //    });

            //migrationBuilder.CreateTable(
            //    name: "OutletTerms",
            //    columns: table => new
            //    {
            //        Id = table.Column<int>(nullable: false)
            //            .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
            //        IsActive = table.Column<bool>(nullable: false),
            //        CreatedBy = table.Column<int>(nullable: true),
            //        UpdatedBy = table.Column<int>(nullable: true),
            //        UpdatedDate = table.Column<DateTime>(nullable: false),
            //        CreatedDate = table.Column<DateTime>(nullable: false),
            //        Label = table.Column<string>(nullable: true),
            //        OutletId = table.Column<int>(nullable: true)
            //    },
            //    constraints: table =>
            //    {
            //        table.PrimaryKey("PK_OutletTerms", x => x.Id);
            //        table.ForeignKey(
            //            name: "FK_OutletTerms_User_CreatedBy",
            //            column: x => x.CreatedBy,
            //            principalTable: "User",
            //            principalColumn: "Id",
            //            onDelete: ReferentialAction.Restrict);
            //        table.ForeignKey(
            //            name: "FK_OutletTerms_Outlets_OutletId",
            //            column: x => x.OutletId,
            //            principalTable: "Outlets",
            //            principalColumn: "Id",
            //            onDelete: ReferentialAction.Restrict);
            //        table.ForeignKey(
            //            name: "FK_OutletTerms_User_UpdatedBy",
            //            column: x => x.UpdatedBy,
            //            principalTable: "User",
            //            principalColumn: "Id",
            //            onDelete: ReferentialAction.Restrict);
            //    });

            //migrationBuilder.CreateTable(
            //    name: "spGetUserActivityLog",
            //    columns: table => new
            //    {
            //        GroupId = table.Column<string>(nullable: false),
            //        Username = table.Column<string>(nullable: true),
            //        Remarks = table.Column<string>(nullable: true),
            //        Total = table.Column<int>(nullable: false)
            //    },
            //    constraints: table =>
            //    {
            //        table.PrimaryKey("PK_spGetUserActivityLog", x => x.GroupId);
            //    });

            migrationBuilder.CreateTable(
                name: "StudentGroupMealPlans",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    IsActive = table.Column<bool>(nullable: false),
                    CreatedBy = table.Column<int>(nullable: true),
                    UpdatedBy = table.Column<int>(nullable: true),
                    UpdatedDate = table.Column<DateTime>(nullable: false),
                    CreatedDate = table.Column<DateTime>(nullable: false),
                    Label = table.Column<string>(nullable: true),
                    Price = table.Column<float>(nullable: false),
                    DeliveryDate = table.Column<DateTime>(nullable: false),
                    MealTypeId = table.Column<int>(nullable: true),
                    StudentGroupId = table.Column<int>(nullable: false),
                    MealSessionId = table.Column<int>(nullable: false),
                    DishId = table.Column<int>(nullable: false),
                    MealSessionDetailId = table.Column<int>(nullable: false),
                    StoreId = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StudentGroupMealPlans", x => x.Id);
                    table.ForeignKey(
                        name: "FK_StudentGroupMealPlans_User_CreatedBy",
                        column: x => x.CreatedBy,
                        principalTable: "User",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_StudentGroupMealPlans_Dishes_DishId",
                        column: x => x.DishId,
                        principalTable: "Dishes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_StudentGroupMealPlans_MealSessionDetails_MealSessionDetailId",
                        column: x => x.MealSessionDetailId,
                        principalTable: "MealSessionDetails",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_StudentGroupMealPlans_StoreInfos_StoreId",
                        column: x => x.StoreId,
                        principalTable: "StoreInfos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_StudentGroupMealPlans_StudentGroups_StudentGroupId",
                        column: x => x.StudentGroupId,
                        principalTable: "StudentGroups",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_StudentGroupMealPlans_User_UpdatedBy",
                        column: x => x.UpdatedBy,
                        principalTable: "User",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "StudentGroupSessions",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    IsActive = table.Column<bool>(nullable: false),
                    CreatedBy = table.Column<int>(nullable: true),
                    UpdatedBy = table.Column<int>(nullable: true),
                    UpdatedDate = table.Column<DateTime>(nullable: false),
                    CreatedDate = table.Column<DateTime>(nullable: false),
                    StudentGroupId = table.Column<int>(nullable: false),
                    MealSessionId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StudentGroupSessions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_StudentGroupSessions_User_CreatedBy",
                        column: x => x.CreatedBy,
                        principalTable: "User",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_StudentGroupSessions_MealSessions_MealSessionId",
                        column: x => x.MealSessionId,
                        principalTable: "MealSessions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_StudentGroupSessions_StudentGroups_StudentGroupId",
                        column: x => x.StudentGroupId,
                        principalTable: "StudentGroups",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_StudentGroupSessions_User_UpdatedBy",
                        column: x => x.UpdatedBy,
                        principalTable: "User",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "UserCaterers",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    IsActive = table.Column<bool>(nullable: false),
                    CreatedBy = table.Column<int>(nullable: true),
                    UpdatedBy = table.Column<int>(nullable: true),
                    UpdatedDate = table.Column<DateTime>(nullable: false),
                    CreatedDate = table.Column<DateTime>(nullable: false),
                    UserId = table.Column<int>(nullable: false),
                    CatererId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserCaterers", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UserCaterers_CatererInfos_CatererId",
                        column: x => x.CatererId,
                        principalTable: "CatererInfos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_UserCaterers_User_CreatedBy",
                        column: x => x.CreatedBy,
                        principalTable: "User",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_UserCaterers_User_UpdatedBy",
                        column: x => x.UpdatedBy,
                        principalTable: "User",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_UserCaterers_User_UserId",
                        column: x => x.UserId,
                        principalTable: "User",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UserOutlets",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    IsActive = table.Column<bool>(nullable: false),
                    CreatedBy = table.Column<int>(nullable: true),
                    UpdatedBy = table.Column<int>(nullable: true),
                    UpdatedDate = table.Column<DateTime>(nullable: false),
                    CreatedDate = table.Column<DateTime>(nullable: false),
                    UserId = table.Column<int>(nullable: false),
                    OutletId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserOutlets", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UserOutlets_User_CreatedBy",
                        column: x => x.CreatedBy,
                        principalTable: "User",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_UserOutlets_Outlets_OutletId",
                        column: x => x.OutletId,
                        principalTable: "Outlets",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_UserOutlets_User_UpdatedBy",
                        column: x => x.UpdatedBy,
                        principalTable: "User",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_UserOutlets_User_UserId",
                        column: x => x.UserId,
                        principalTable: "User",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            //migrationBuilder.CreateTable(
            //    name: "OrderPortalBanners",
            //    columns: table => new
            //    {
            //        Id = table.Column<int>(nullable: false)
            //            .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
            //        IsActive = table.Column<bool>(nullable: false),
            //        CreatedBy = table.Column<int>(nullable: true),
            //        UpdatedBy = table.Column<int>(nullable: true),
            //        UpdatedDate = table.Column<DateTime>(nullable: false),
            //        CreatedDate = table.Column<DateTime>(nullable: false),
            //        Url = table.Column<string>(nullable: true),
            //        Title = table.Column<string>(nullable: true),
            //        Subtitle = table.Column<string>(nullable: true),
            //        Order = table.Column<int>(nullable: false),
            //        ImageId = table.Column<int>(nullable: true),
            //        OutletId = table.Column<int>(nullable: true),
            //        OrderPortalContentId = table.Column<int>(nullable: true),
            //        FileName = table.Column<string>(nullable: true),
            //        FilePath = table.Column<string>(nullable: true),
            //        ImageFileName = table.Column<string>(nullable: true),
            //        ImageFilePath = table.Column<string>(nullable: true)
            //    },
            //    constraints: table =>
            //    {
            //        table.PrimaryKey("PK_OrderPortalBanners", x => x.Id);
            //        table.ForeignKey(
            //            name: "FK_OrderPortalBanners_User_CreatedBy",
            //            column: x => x.CreatedBy,
            //            principalTable: "User",
            //            principalColumn: "Id",
            //            onDelete: ReferentialAction.Restrict);
            //        table.ForeignKey(
            //            name: "FK_OrderPortalBanners_Files_ImageId",
            //            column: x => x.ImageId,
            //            principalTable: "Files",
            //            principalColumn: "Id",
            //            onDelete: ReferentialAction.Restrict);
            //        table.ForeignKey(
            //            name: "FK_OrderPortalBanners_OrderPortalContents_OrderPortalContentId",
            //            column: x => x.OrderPortalContentId,
            //            principalTable: "OrderPortalContents",
            //            principalColumn: "Id",
            //            onDelete: ReferentialAction.Restrict);
            //        table.ForeignKey(
            //            name: "FK_OrderPortalBanners_User_UpdatedBy",
            //            column: x => x.UpdatedBy,
            //            principalTable: "User",
            //            principalColumn: "Id",
            //            onDelete: ReferentialAction.Restrict);
            //    });

            //migrationBuilder.CreateIndex(
            //    name: "IX_StudentGroups_OutletTermId",
            //    table: "StudentGroups",
            //    column: "OutletTermId");

            migrationBuilder.CreateIndex(
                name: "IX_MealPlanOrders_CreatedBy",
                table: "MealPlanOrders",
                column: "CreatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_MealPlanOrders_PaymentId",
                table: "MealPlanOrders",
                column: "PaymentId");

            migrationBuilder.CreateIndex(
                name: "IX_MealPlanOrders_ProfileId",
                table: "MealPlanOrders",
                column: "ProfileId");

            migrationBuilder.CreateIndex(
                name: "IX_MealPlanOrders_StoreId",
                table: "MealPlanOrders",
                column: "StoreId");

            migrationBuilder.CreateIndex(
                name: "IX_MealPlanOrders_StudentGroupId",
                table: "MealPlanOrders",
                column: "StudentGroupId");

            migrationBuilder.CreateIndex(
                name: "IX_MealPlanOrders_UpdatedBy",
                table: "MealPlanOrders",
                column: "UpdatedBy");

            //migrationBuilder.CreateIndex(
            //    name: "IX_OrderPortalBanners_CreatedBy",
            //    table: "OrderPortalBanners",
            //    column: "CreatedBy");

            //migrationBuilder.CreateIndex(
            //    name: "IX_OrderPortalBanners_ImageId",
            //    table: "OrderPortalBanners",
            //    column: "ImageId");

            //migrationBuilder.CreateIndex(
            //    name: "IX_OrderPortalBanners_OrderPortalContentId",
            //    table: "OrderPortalBanners",
            //    column: "OrderPortalContentId");

            //migrationBuilder.CreateIndex(
            //    name: "IX_OrderPortalBanners_UpdatedBy",
            //    table: "OrderPortalBanners",
            //    column: "UpdatedBy");

            //migrationBuilder.CreateIndex(
            //    name: "IX_OrderPortalContents_CreatedBy",
            //    table: "OrderPortalContents",
            //    column: "CreatedBy");

            //migrationBuilder.CreateIndex(
            //    name: "IX_OrderPortalContents_OutletId",
            //    table: "OrderPortalContents",
            //    column: "OutletId");

            //migrationBuilder.CreateIndex(
            //    name: "IX_OrderPortalContents_UpdatedBy",
            //    table: "OrderPortalContents",
            //    column: "UpdatedBy");

            //migrationBuilder.CreateIndex(
            //    name: "IX_OutletTerms_CreatedBy",
            //    table: "OutletTerms",
            //    column: "CreatedBy");

            //migrationBuilder.CreateIndex(
            //    name: "IX_OutletTerms_OutletId",
            //    table: "OutletTerms",
            //    column: "OutletId");

            //migrationBuilder.CreateIndex(
            //    name: "IX_OutletTerms_UpdatedBy",
            //    table: "OutletTerms",
            //    column: "UpdatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_StudentGroupMealPlans_CreatedBy",
                table: "StudentGroupMealPlans",
                column: "CreatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_StudentGroupMealPlans_DishId",
                table: "StudentGroupMealPlans",
                column: "DishId");

            migrationBuilder.CreateIndex(
                name: "IX_StudentGroupMealPlans_MealSessionDetailId",
                table: "StudentGroupMealPlans",
                column: "MealSessionDetailId");

            migrationBuilder.CreateIndex(
                name: "IX_StudentGroupMealPlans_StoreId",
                table: "StudentGroupMealPlans",
                column: "StoreId");

            migrationBuilder.CreateIndex(
                name: "IX_StudentGroupMealPlans_StudentGroupId",
                table: "StudentGroupMealPlans",
                column: "StudentGroupId");

            migrationBuilder.CreateIndex(
                name: "IX_StudentGroupMealPlans_UpdatedBy",
                table: "StudentGroupMealPlans",
                column: "UpdatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_StudentGroupSessions_CreatedBy",
                table: "StudentGroupSessions",
                column: "CreatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_StudentGroupSessions_MealSessionId",
                table: "StudentGroupSessions",
                column: "MealSessionId");

            migrationBuilder.CreateIndex(
                name: "IX_StudentGroupSessions_StudentGroupId",
                table: "StudentGroupSessions",
                column: "StudentGroupId");

            migrationBuilder.CreateIndex(
                name: "IX_StudentGroupSessions_UpdatedBy",
                table: "StudentGroupSessions",
                column: "UpdatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_UserCaterers_CatererId",
                table: "UserCaterers",
                column: "CatererId");

            migrationBuilder.CreateIndex(
                name: "IX_UserCaterers_CreatedBy",
                table: "UserCaterers",
                column: "CreatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_UserCaterers_UpdatedBy",
                table: "UserCaterers",
                column: "UpdatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_UserCaterers_UserId",
                table: "UserCaterers",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_UserOutlets_CreatedBy",
                table: "UserOutlets",
                column: "CreatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_UserOutlets_OutletId",
                table: "UserOutlets",
                column: "OutletId");

            migrationBuilder.CreateIndex(
                name: "IX_UserOutlets_UpdatedBy",
                table: "UserOutlets",
                column: "UpdatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_UserOutlets_UserId",
                table: "UserOutlets",
                column: "UserId");

            //migrationBuilder.AddForeignKey(
            //    name: "FK_StudentGroups_OutletTerms_OutletTermId",
            //    table: "StudentGroups",
            //    column: "OutletTermId",
            //    principalTable: "OutletTerms",
            //    principalColumn: "Id",
            //    onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_StudentGroups_OutletTerms_OutletTermId",
                table: "StudentGroups");

            migrationBuilder.DropTable(
                name: "MealPlanOrders");

            migrationBuilder.DropTable(
                name: "OrderPortalBanners");

            migrationBuilder.DropTable(
                name: "OutletTerms");

            migrationBuilder.DropTable(
                name: "spGetUserActivityLog");

            migrationBuilder.DropTable(
                name: "StudentGroupMealPlans");

            migrationBuilder.DropTable(
                name: "StudentGroupSessions");

            migrationBuilder.DropTable(
                name: "UserCaterers");

            migrationBuilder.DropTable(
                name: "UserOutlets");

            migrationBuilder.DropTable(
                name: "OrderPortalContents");

            migrationBuilder.DropIndex(
                name: "IX_StudentGroups_OutletTermId",
                table: "StudentGroups");

            migrationBuilder.DropColumn(
                name: "Last2FAValidatedTime",
                table: "User");

            migrationBuilder.DropColumn(
                name: "AmendReason",
                table: "TokenOrders");

            migrationBuilder.DropColumn(
                name: "IsMealPlan",
                table: "TokenOrders");

            migrationBuilder.DropColumn(
                name: "IsStudentGroupOrder",
                table: "TokenOrders");

            migrationBuilder.DropColumn(
                name: "DeliveryEndDate",
                table: "StudentGroups");

            migrationBuilder.DropColumn(
                name: "DeliveryStartDate",
                table: "StudentGroups");

            migrationBuilder.DropColumn(
                name: "Description",
                table: "StudentGroups");

            migrationBuilder.DropColumn(
                name: "EndDate",
                table: "StudentGroups");

            migrationBuilder.DropColumn(
                name: "FileName",
                table: "StudentGroups");

            migrationBuilder.DropColumn(
                name: "FilePath",
                table: "StudentGroups");

            migrationBuilder.DropColumn(
                name: "IsPublished",
                table: "StudentGroups");

            migrationBuilder.DropColumn(
                name: "OutletTermId",
                table: "StudentGroups");

            migrationBuilder.DropColumn(
                name: "Price",
                table: "StudentGroups");

            migrationBuilder.DropColumn(
                name: "Sequence",
                table: "StudentGroups");

            migrationBuilder.DropColumn(
                name: "StartDate",
                table: "StudentGroups");

            migrationBuilder.DropColumn(
                name: "Term",
                table: "StudentGroups");

            migrationBuilder.DropColumn(
                name: "Type",
                table: "StudentGroups");

            migrationBuilder.DropColumn(
                name: "StudentGroupName",
                table: "spSalesOrderReport");

            migrationBuilder.DropColumn(
                name: "ActionName",
                table: "AuditLogs");

            migrationBuilder.DropColumn(
                name: "GroupId",
                table: "AuditLogs");

            migrationBuilder.DropColumn(
                name: "Remarks",
                table: "AuditLogs");
        }
    }
}
