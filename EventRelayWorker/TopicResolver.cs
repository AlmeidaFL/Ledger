namespace EventRelayWorker;

public class TopicResolver
{
    public static string ResolveTopic(string service, string type, string? topicOverride)
    {
        if (!string.IsNullOrWhiteSpace(topicOverride))
        {
            return topicOverride.ToLowerInvariant().Trim();
        }

        if (string.IsNullOrWhiteSpace(service))
        {
            throw new ArgumentNullException(nameof(service), "Service name cannot be null or empty.");
        }
        
        if (string.IsNullOrWhiteSpace(type))
        {
            throw new ArgumentNullException(nameof(type), "Event Type name cannot be null or empty.");
        }
        
        return $"{Normalize(service)}.{Normalize(type)}";
    }
    
    private static string Normalize(string input)
        => input.Trim().Replace(" ", "").ToLowerInvariant();
}