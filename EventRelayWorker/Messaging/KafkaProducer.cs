using Confluent.Kafka;
using Microsoft.Extensions.Options;

namespace EventRelayWorker.Messaging;

public interface IKafkaProducer
{
    Task ProduceAsync(string topic, string key, string value, CancellationToken cancellationToken = default);
}

public class KafkaProducer : IKafkaProducer, IDisposable
{
    private readonly IProducer<string, string> producer;
    private readonly ILogger<KafkaProducer> logger;

    public KafkaProducer(IOptions<KafkaOptions> options, ILogger<KafkaProducer> logger)
    {
        this.logger = logger;
        
        var config = new ProducerConfig
        {
            BootstrapServers = options.Value.BootstrapServers,
            EnableIdempotence = options.Value.EnableIdempotence,
            Acks = Acks.All,
            RetryBackoffMs = 500,
            MessageSendMaxRetries = 5,
        };
        
        producer = new ProducerBuilder<string, string>(config)
            .SetErrorHandler((_, error) =>
            {
                logger.LogError(
                    "Kafka producer error: code={Code}, reason={Reason}, isFatal={IsFatal}, isLocal={IsLocal}",
                    error.Code,
                    error.Reason,
                    error.IsFatal,
                    error.IsLocalError
                );
            })
            .Build();
    }
    
    public async Task ProduceAsync(string topic, string key, string value, CancellationToken cancellationToken = default)
    {
        var message = new Message<string, string>
        {
            Key = key,
            Value = value
        };
        
        await producer.ProduceAsync(topic, message, cancellationToken);
    }

    public void Dispose()
    {
        producer.Dispose();
    }
}