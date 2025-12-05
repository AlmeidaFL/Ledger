using System.Text.Json;
using Confluent.Kafka;
using Microsoft.Extensions.Options;
using UserApi.Messaging.Events;

namespace UserApi.Messaging;

public interface IKafkaDispatcher
{
    Task DispatchAsync(
        ConsumeResult<string, string> message,
        CancellationToken ct);
}

public class KafkaDispatcher(
    ILogger<KafkaDispatcher> logger,
    IServiceProvider provider,
    IOptions<KafkaSettings> kafkaOptions)
    : IKafkaDispatcher
{
    private const string UserCreatedTopic = "simple-auth.registered-user";
    private const string FinancialAccountCreatedTopic = "financial-service.account-balance-created";
    
    private readonly KafkaSettings settings = kafkaOptions.Value;

    public async Task DispatchAsync(
        ConsumeResult<string, string> message,
        CancellationToken ct)
    {
        if (message.Topic == UserCreatedTopic)
        {
            var evt = Deserialize<UserCreatedEvent>(message.Message.Value);
            var handler = provider.GetRequiredService<IMessageHandler<UserCreatedEvent>>();
            await handler.HandleAsync(evt, ct);
            return;
        }

        if (message.Topic == FinancialAccountCreatedTopic)
        {
            var evt = Deserialize<FinancialAccountCreatedEvent>(message.Message.Value);
            var handler = provider.GetRequiredService<IMessageHandler<FinancialAccountCreatedEvent>>();
            await handler.HandleAsync(evt, ct);
            return;
        }

        logger.LogWarning("Received message for unknown topic {Topic}", message.Topic);
    }

    private static T Deserialize<T>(string json)
    {
        try
        {
            return JsonSerializer.Deserialize<T>(json)
                   ?? throw new InvalidOperationException("Payload deserialized to null");
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException(
                $"Error deserializing event {typeof(T).Name}: {ex.Message}", ex);
        }
    }
}