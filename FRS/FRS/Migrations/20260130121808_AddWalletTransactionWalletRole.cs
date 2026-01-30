using Microsoft.EntityFrameworkCore.Migrations;

namespace FRS.Migrations
{
    /// <inheritdoc />
    public partial class AddWalletTransactionWalletRole : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
                DECLARE @Role VARCHAR(50)
                SELECT @Role = Id FROM [Role] WHERE Name = 'CAT_ADMIN'

                IF NOT EXISTS (SELECT * FROM UserRoleClaim WHERE ClaimValue = 'mosmgt.reportmgt.wallettransaction.view')
                BEGIN
	                INSERT INTO UserRoleClaim (RoleId, ClaimType, ClaimValue) VALUES
	                (@Role, 'permission', 'mosmgt.reportmgt.wallettransaction.view')
                END
            ");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {

        }
    }
}
