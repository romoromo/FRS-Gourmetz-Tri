using Microsoft.EntityFrameworkCore.Migrations;

namespace FRS.Migrations
{
    /// <inheritdoc />
    public partial class AddFasMonthlyReport : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
               DECLARE @Role VARCHAR(50)
               SELECT @Role = Id FROM [Role] WHERE Name = 'CAT_ADMIN'

               IF NOT EXISTS (SELECT * FROM UserRoleClaim WHERE ClaimValue = 'mosmgt.reportmgt.fasmonthlybillingreport.view')
               BEGIN
	               INSERT INTO UserRoleClaim (RoleId, ClaimType, ClaimValue) VALUES
	               (@Role, 'permission', 'mosmgt.reportmgt.fasmonthlybillingreport.view')
               END

            IF NOT EXISTS (SELECT * FROM sys.indexes 
               WHERE name = 'IX_StudentWalletTransactions_FASReport' 
               AND object_id = OBJECT_ID('StudentWalletTransactions'))
            BEGIN
                CREATE INDEX IX_StudentWalletTransactions_FASReport 
                ON StudentWalletTransactions (IsActive, CreatedDate DESC)
                INCLUDE (TransactionType, StudentId, PaymentId);
    
                PRINT 'Index IX_StudentWalletTransactions_FASReport created successfully.';
            END
            ");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {

        }
    }
}
