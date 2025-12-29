using FinancialService.Model;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FinancialService.Migrations
{
    /// <inheritdoc />
    public partial class SeedCashInOutAccount : Migration
    {
        private static readonly string[] accountColumns = new[]
                {
                    "Id",
                    "UserId",
                    "Currency",
                    "CreatedAt"
                };
        
        private static readonly string[] userColumns = new[]
        {
            "Id",
            "Email",
            "Name",
            "CreatedAt"
        };

        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<long>(
                name: "Amount",
                table: "JournalEntries",
                type: "bigint",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer");

            
            migrationBuilder.InsertData(
                table: "Users",
                columns: userColumns,
                values: new object[]
                {
                    SystemAccounts.CashInAccountId,
                    "cashin@email.com",
                    "CashInUser",
                    DateTime.UtcNow
                });
            
            migrationBuilder.InsertData(
                table: "Accounts",
                columns: accountColumns,
                values: new object[]
                {
                    SystemAccounts.CashInAccountId,
                    SystemAccounts.CashInAccountId,
                    "BRL",
                    DateTime.UtcNow
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "Amount",
                table: "JournalEntries",
                type: "integer",
                nullable: false,
                oldClrType: typeof(long),
                oldType: "bigint");
        }
    }
}
