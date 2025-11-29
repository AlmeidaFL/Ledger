using Microsoft.EntityFrameworkCore;

namespace EventRelayWorker.Data;

public class OutboxDbContextFactory
{
    public OutboxDbContext CreateDbContext(string connectionString, string tableName)
    {
        var options = new DbContextOptionsBuilder<OutboxDbContext>()
            .UseNpgsql(connectionString)
            .Options;
        return new OutboxDbContext(options, tableName);
    }
}