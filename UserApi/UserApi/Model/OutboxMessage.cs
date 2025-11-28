
using System.Text.Json;

namespace UserApi.Model;

public class OutboxMessage
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Type { get; set; }
    public string Payload { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? ProcessedAt { get; set; }
    public string? Error { get; set; }

    public static OutboxMessage FromEvent<T>(T message)
    {
        return new OutboxMessage
        {
            Type = nameof(T),
            Payload = JsonSerializer.Serialize(message),
        };
    }
}