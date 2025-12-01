namespace UserApi.Model;

public class ProcessedEvent
{
    public required string IdempotencyKey { get; set; }
    public DateTime ProcessedAt { get; set; }
}