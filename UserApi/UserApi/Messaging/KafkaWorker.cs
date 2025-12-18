using System.Text.Json;
using Confluent.Kafka;
using Microsoft.Extensions.Options;
using UserApi.Messaging.Events;
using UserApi.Messaging.Handlers;

namespace UserApi.Messaging;

public class KafkaWorker(
    ILogger<KafkaWorker> logger,
    IServiceScopeFactory scopeFactory,
    IOptions<KafkaSettings> settings)
    : BackgroundService
{
    private readonly KafkaSettings _settings = settings.Value;

    private const string UserCreatedTopic = "simple-auth.registered-user";
    private const string FinancialAccountCreatedTopic = "financial-service.account-balance-created";
    
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await Task.Yield();
        
        var config = new ConsumerConfig
        {
            BootstrapServers = _settings.BootstrapServers,
            GroupId = _settings.GroupId,
            AutoOffsetReset = AutoOffsetReset.Earliest,
            EnableAutoCommit = false,
        };

        using var consumer = new ConsumerBuilder<string, string>(config).Build();

        consumer.Subscribe(new[]
        {
            UserCreatedTopic,
            FinancialAccountCreatedTopic
        });

        logger.LogInformation("KafkaWorker subscribed to: {Topics}",
            string.Join(", ", consumer.Subscription));

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                var result = consumer.Consume(stoppingToken);

                logger.LogInformation(
                    "Kafka message received: Topic={Topic}, Key={Key}, Partition={Partition}, Offset={Offset}",
                    result.Topic,
                    result.Message.Key,
                    result.Partition,
                    result.Offset);

                using var scope = scopeFactory.CreateScope();
                var dispatcher = scope.ServiceProvider.GetRequiredService<IKafkaDispatcher>();

                await dispatcher.DispatchAsync(result, stoppingToken);

                consumer.Commit(result);
            }
            catch (ConsumeException ex)
            {
                logger.LogError(ex, "Kafka consume error: {Reason}", ex.Error.Reason);
                await Task.Delay(2000, stoppingToken);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Unhandled exception in KafkaWorker");
            }
        }

        consumer.Close();
        logger.LogInformation("KafkaWorker shutting down.");
    }
}