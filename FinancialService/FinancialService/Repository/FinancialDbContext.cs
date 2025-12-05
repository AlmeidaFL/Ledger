using FinancialService.Model;
using Microsoft.EntityFrameworkCore;
using ServiceCommons;

namespace FinancialService.Repository;

public class FinancialDbContext(DbContextOptions<FinancialDbContext> options) : DbContext(options)
{
    public DbSet<Account> Accounts { get; set; }
    public DbSet<Transaction> Transactions { get; set; }
    public DbSet<User> Users { get; set; }
    public DbSet<JournalEntry> JournalEntries { get; set; }
    public DbSet<OutboxMessage> OutboxMessages { get; set; }
    public DbSet<AccountLock> AccountLocks { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(FinancialDbContext).Assembly);
    }
}