namespace EventRelayWorker;

public class KafkaOptions
{
    public string BootstrapServers { get; set; }
    public string ClientId { get; set; }
    public bool EnableIdempotence { get; set; } = true;
}