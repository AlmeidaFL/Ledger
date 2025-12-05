using EventRelayWorker.Data;
using EventRelayWorker.Messaging;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.Extensions.Options;
using ServiceCommons;

namespace EventRelayWorker;

public class EventRelayWorker(
    ILogger<EventRelayWorker> logger,
    IOptions<Tenants> tenants,
    OutboxDbContextFactory factory,
    IKafkaProducer kafkaProducer,
    IOptions<OutboxOptions> outboxOptions) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken cancellationToken)
    {
        while (!cancellationToken.IsCancellationRequested)
        {
            foreach (var tenant in tenants.Value.TenantConfigs)
            {
                try
                {
                    await using var db =
                        factory.CreateDbContext(tenant.ConnectionString, tenant.OutboxTable);
                    var tableName = tenant.OutboxTable;
                    var sql = $@"
                        SELECT *
                        FROM ""{tableName}""
                        WHERE ""ProcessedAt"" IS NULL
                        ORDER BY ""CreatedAt""
                        FOR UPDATE SKIP LOCKED
                        LIMIT {outboxOptions.Value.BatchSize};
                    ";

                    var messages = await db.OutboxMessages
                        .FromSqlRaw(sql)
                        .ToListAsync(cancellationToken);
                    
                    if (messages.Count == 0)
                        continue;

                    foreach (var message in messages)
                    {
                        try
                        {
                            await kafkaProducer.ProduceAsync(
                                topic: message.Topic,
                                key: message.Id.ToString(),
                                value: message.Payload,
                                cancellationToken);

                            message.ProcessedAt = DateTime.UtcNow;
                            message.Error = null;

                            await db.SaveChangesAsync(cancellationToken);
                        }
                        catch (Exception ex)
                        {
                            message.RetryCount++;
                            message.Error = ex.Message;
                            
                            logger.LogError(ex,
                                "Failed to send outbox message {Id} for tenant {Tenant}",
                                message.Id,
                                message.ServiceOriginName);
                        }
                    }

                }
                catch (Exception ex)
                {
                    logger.LogError(
                        ex,
                        "Worker error while processing tenant {Tenant}",
                        tenant.Name);
                }
            }
            
            await Task.Delay(TimeSpan.FromSeconds(outboxOptions.Value.PollingIntervalSeconds), cancellationToken);
        }
    }
}
