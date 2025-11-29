namespace EventRelayWorker;

public class OutboxOptions
{
    public int BatchSize { get; set; }
    public int PollingIntervalSeconds { get; set; }
    public int MaxRetries { get; set; }
}