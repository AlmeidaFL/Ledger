using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FinancialService.Migrations
{
    /// <inheritdoc />
    public partial class AddAccountLock : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AccountLocks",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AccountLocks", x => x.Id);
                });
            
            migrationBuilder.Sql(@"
                CREATE OR REPLACE FUNCTION create_account_lock()
                RETURNS trigger AS $$
                BEGIN
                    INSERT INTO ""AccountLocks""(""Id"")
                    VALUES (NEW.""Id"")
                    ON CONFLICT DO NOTHING;
                    RETURN NEW;
                END;
                $$ LANGUAGE plpgsql;
            ");

            migrationBuilder.Sql(@"
                CREATE TRIGGER account_lock_trigger
                AFTER INSERT ON ""Accounts""
                FOR EACH ROW
                EXECUTE FUNCTION create_account_lock();
            ");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AccountLocks");
            
            migrationBuilder.Sql(@"DROP TRIGGER IF EXISTS account_lock_trigger ON ""Accounts"";");
            migrationBuilder.Sql(@"DROP FUNCTION IF EXISTS create_account_lock();");
        }
    }
}
