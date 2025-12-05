using System.Text.Json;

namespace ServiceCommons;

public interface IOutboxEvent
{
    /// <summary>
    /// It should be a GUID version 7
    /// </summary>
    public Guid Id { get; init; }
    public Guid AggregateId { get; init; }
}

public class OutboxMessage
{
    public Guid Id { get; set; }
    
    public string ServiceOriginName { get; set; }
    public string Type { get; set; }
    public string? Topic { get; set; }
    public string Payload { get; set; }
    public Guid AggregateId { get; set; }
    
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? ProcessedAt { get; set; }
    
    public string? Error { get; set; }
    public int RetryCount { get; set; }

    public static OutboxMessage FromEvent<T>(
        T message,
        string serviceOriginName,
        string? topic = null)
        where T : IOutboxEvent
    {
        return new OutboxMessage
        {
            Id = message.Id,
            ServiceOriginName = serviceOriginName,
            Type = nameof(T),
            AggregateId = message.AggregateId,
            Topic = topic,
            Payload = JsonSerializer.Serialize(message),
        };
    }
}