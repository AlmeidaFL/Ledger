using Confluent.Kafka;
using Confluent.Kafka.Admin;
using Microsoft.Extensions.Logging;

namespace EventRelayWorker.Messaging;

public class KafkaBootstrapper(string bootstrapServers, ILogger<KafkaBootstrapper> logger)
{
    private readonly List<TopicSpecification> _topics = [];

    public KafkaBootstrapper AddTopic(string topicName, int partitions = 1, short replicationFactor = 1)
    {
        _topics.Add(new TopicSpecification
        {
            Name = topicName,
            NumPartitions = partitions,
            ReplicationFactor = replicationFactor
        });

        return this;
    }

    public async Task WaitForKafkaAsync(CancellationToken ct = default)
    {
        using var admin = new AdminClientBuilder(new AdminClientConfig
        {
            BootstrapServers = bootstrapServers
        }).Build();

        while (!ct.IsCancellationRequested)
        {
            try
            {
                admin.GetMetadata(TimeSpan.FromSeconds(5));
                logger.LogInformation("Kafka is ready.");
                return;
            }
            catch (Exception)
            {
                logger.LogWarning("Kafka is not ready yet. Retrying in 3s...");
                await Task.Delay(3000, ct);
            }
        }
    }

    public async Task CreateTopicsAsync(CancellationToken ct = default)
    {
        if (!_topics.Any())
        {
            logger.LogWarning("No topics registered for creation.");
            return;
        }

        using var admin = new AdminClientBuilder(new AdminClientConfig
        {
            BootstrapServers = bootstrapServers
        }).Build();

        try
        {
            logger.LogInformation("Creating Kafka topics: {Topics}", string.Join(", ", _topics.Select(t => t.Name)));
            await admin.CreateTopicsAsync(_topics, new CreateTopicsOptions { RequestTimeout = TimeSpan.FromSeconds(10) });
            logger.LogInformation("Kafka topics created successfully.");
        }
        catch (CreateTopicsException ex)
        {
            foreach (var result in ex.Results)
            {
                if (result.Error.Code == ErrorCode.TopicAlreadyExists)
                {
                    logger.LogInformation("Kafka topic already exists: {Topic}", result.Topic);
                }
                else
                {
                    logger.LogError("Failed to create Kafka topic {Topic}: {Error}", result.Topic, result.Error);
                }
            }
        }
    }

    public async Task InitializeAsync(CancellationToken ct = default)
    {
        logger.LogInformation("Initializing Kafka bootstrap process...");

        await WaitForKafkaAsync(ct);
        await CreateTopicsAsync(ct);

        logger.LogInformation("Kafka bootstrap completed.");
        
        WorkerHealth.Ready = true;
    }
}
