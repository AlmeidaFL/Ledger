namespace EventRelayWorker.Data;

public class OutboxMessage
{
    public Guid Id { get; set; }
    public string ServiceOriginName { get; set; } = default!;
    public string Type { get; set; } = default!;
    public string Payload { get; set; } = default!;
    public string? Topic { get; set; }
    public string? AggregateId { get; set; }
    public int RetryCount { get; set; }
    public string? Error { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? ProcessedAt { get; set; }
}