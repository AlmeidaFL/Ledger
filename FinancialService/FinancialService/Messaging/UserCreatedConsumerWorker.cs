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
            AutoOffsetReset = AutoOffsetReset.Latest,
            EnableAutoCommit = false,
        };
        
        using var consumer = new ConsumerBuilder<string, string>(config).Build();

        var topics = new[]
        {
            "simple-auth.registered-user",
        };
        consumer.Subscribe(topics);
        
        logger.LogInformation("Consumer subscribed to topic: {Topic}", string.Join(",", topics));
        
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
                    consumer.Commit(result);
                    continue;
                }
        
                await HandleMessage(user, stoppingToken);
        
                consumer.Commit(result);
            }
            catch (ConsumeException ex) when (ex.Error.Code == ErrorCode.UnknownTopicOrPart)
            {
                logger.LogError(ex, "Topic not ready yet. Retrying in 5 seconds...");
                await Task.Delay(5000, stoppingToken);
                continue;
            }
            catch (ConsumeException ex)
            {
                logger.LogError(ex, "Error occured while consuming message");
                break;
            }
            
            await Task.Delay(5000, stoppingToken);
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

    private async Task HandleMessage(UserCreatedEvent user, CancellationToken stoppingToken = default)
    {
        using var scope = scopeFactory.CreateScope();
        var handler = scope.ServiceProvider.GetRequiredService<IUserCreatedHandler>();
        await handler.HandleAsync(user, stoppingToken);
    }
}