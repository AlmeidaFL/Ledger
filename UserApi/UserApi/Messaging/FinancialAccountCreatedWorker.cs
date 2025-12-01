using System.Text.Json;
using Confluent.Kafka;
using Microsoft.Extensions.Options;
using UserApi.Application.Handlers;
using UserApi.Messaging.Events;

namespace UserApi.Messaging;

public class FinancialAccountCreatedWorker(
    ILogger<FinancialAccountCreatedWorker> logger,
    IOptions<KafkaFinancialAccountCreatedSettings> kafkaSettings,
    IServiceScopeFactory factory) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var consumerSettings = new ConsumerConfig
        {
            BootstrapServers = kafkaSettings.Value.BootstrapServers,
            GroupId = kafkaSettings.Value.GroupId,
            AutoOffsetReset = AutoOffsetReset.Latest,
            EnableAutoCommit = false,
        };
        
        using var consumer = new ConsumerBuilder<string, string>(consumerSettings).Build();
        
        consumer.Subscribe(kafkaSettings.Value.Topic);
        
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
        
                var account = GetAccount(result.Message.Value);
                if (account is null)
                {
                    logger.LogWarning("Deserialized event is null");
                    // consumer.Commit(result);
                    continue;
                }
        
                await HandleMessage(account, stoppingToken);
                
                // consumer.Commit(result);
            }
            catch (ConsumeException ex)
            {
                logger.LogError(ex, "Error occured while consuming message");
                break;
            }
        }
        
        consumer.Close();
        logger.LogInformation($"{nameof(FinancialAccountCreatedEvent)} shutting down");
        await Task.Delay(5000, stoppingToken);
    }

    private FinancialAccountCreatedEvent? GetAccount(string value)
    {
        FinancialAccountCreatedEvent? evt = null;
        try
        {
            evt = JsonSerializer.Deserialize<FinancialAccountCreatedEvent>(value);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Invalid JSON in message on {Worker}", nameof(FinancialAccountCreatedEvent));
        }

        return evt;
    }
    
    private async Task HandleMessage(FinancialAccountCreatedEvent account, CancellationToken stoppingToken)
    {
        using var scope = factory.CreateScope();
        var handler = scope.ServiceProvider.GetRequiredService<IFinancialAccountCreatedHandler>();
        await handler.HandleAsync(account, stoppingToken);
    }
}