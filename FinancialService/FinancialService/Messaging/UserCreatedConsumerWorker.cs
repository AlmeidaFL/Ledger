using System.Text.Json;
using Confluent.Kafka;
using FinancialService.Application;
using FinancialService.Messaging.Events;
using Microsoft.Extensions.Options;

namespace FinancialService.Messaging;

public class UserCreatedConsumerWorker(
    ILogger<UserCreatedConsumerWorker> logger,
    IOptions<KafkaUserCreatedConsumerSettings> options,
    IServiceScopeFactory scopeFactory) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var config = new ConsumerConfig
        {
            BootstrapServers = options.Value.BootstrapServers,
            GroupId = options.Value.GroupId,
            AutoOffsetReset = AutoOffsetReset.Earliest,
            EnableAutoCommit = false,
        };
        
        using var consumer = new ConsumerBuilder<string, string>(config).Build();
        
        consumer.Subscribe(options.Value.Topic);
        
        logger.LogInformation("Consumer subscribed to topic: {Topic}", options.Value.Topic);

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                var result = consumer.Consume(stoppingToken);

                logger.LogInformation(
                    "Received message. Key={Key}, Value={Value}, Partition={Partition}, Offset={Offset}",
                    result.Message.Key,
                    result.Message.Value,
                    result.Partition,
                    result.Offset);

                var user = GetUser(result.Message.Value);
                if (user is null)
                {
                    logger.LogWarning("Deserialized event is null");
                    // consumer.Commit(result);
                    continue;
                }

                using var scope = scopeFactory.CreateScope();
                var handler = scope.ServiceProvider.GetRequiredService<IUserCreatedHandler>();
                await handler.HandleAsync(user, stoppingToken);
                
                // consumer.Commit(result);
            }
            catch (ConsumeException ex)
            {
                logger.LogError(ex, "Error occured while consuming message");
                break;
            }
        }
        
        consumer.Close();
        logger.LogInformation("UserCreatedConsumerWorker shutting down");
    }

    private UserCreatedEvent? GetUser(string value)
    {
        UserCreatedEvent? evt = null;
        try
        {
            evt = JsonSerializer.Deserialize<UserCreatedEvent>(value);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Invalid JSON in message on {Worker}", nameof(UserCreatedConsumerWorker));
        }

        return evt;
    }
}