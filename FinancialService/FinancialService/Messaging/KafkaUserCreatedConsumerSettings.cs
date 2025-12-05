namespace FinancialService.Messaging;

public class KafkaUserCreatedConsumerSettings
{
    public string BootstrapServers { get; set; }
    public string GroupId { get; set; }
}