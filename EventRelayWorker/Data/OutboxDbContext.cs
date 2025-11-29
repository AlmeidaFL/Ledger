using Microsoft.EntityFrameworkCore;

namespace EventRelayWorker.Data;

public class OutboxDbContext(DbContextOptions<OutboxDbContext> options, string tableName) : DbContext(options)
{
    private readonly string tableName = tableName;
    
    public DbSet<OutboxMessage> OutboxMessages => Set<OutboxMessage>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<OutboxMessage>(b =>
        {
            b.ToTable(tableName);
            b.HasKey(x => x.Id);
        });
    }
}