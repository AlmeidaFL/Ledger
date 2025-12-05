using Microsoft.EntityFrameworkCore;
using ServiceCommons;

namespace EventRelayWorker.Data;

public class OutboxDbContext(DbContextOptions<OutboxDbContext> options, string tableName) : DbContext(options)
{
    private readonly string tableName = tableName;
    
    public DbSet<OutboxMessage> OutboxMessages => Set<OutboxMessage>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<OutboxMessage>(b =>
        {
            b.HasKey(x => x.Id);
            b.Property(x => x.Payload)
                .HasColumnType("jsonb");
            b.Property(x => x.CreatedAt)
                .HasDefaultValueSql("NOW()");
            b.HasIndex(x => x.ProcessedAt);
        });
    }
}