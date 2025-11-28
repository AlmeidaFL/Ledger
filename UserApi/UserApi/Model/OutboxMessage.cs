
using System.Text.Json;
using UserApi.Services.Events;

namespace UserApi.Model;

public class OutboxMessage
{
    public const string UserCreatedTopic = "UserCreated";
    
    public Guid Id { get; set; } = Guid.NewGuid();
    
    public string ServiceOriginName { get; set; }
    public string Type { get; set; }
    public string Topic { get; set; }
    public string Payload { get; set; }
    public string AggregateId { get; set; }
    
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? ProcessedAt { get; set; }
    
    public string? Error { get; set; }
    public int RetryCount { get; set; }

    public static OutboxMessage FromEvent<T>(T message, string topic)
        where T : OutboxEvent
    {
        return new OutboxMessage
        {
            ServiceOriginName = "user-api",
            Type = nameof(T),
            AggregateId = message.Id,
            Topic = topic,
            Payload = JsonSerializer.Serialize(message),
        };
    }
}