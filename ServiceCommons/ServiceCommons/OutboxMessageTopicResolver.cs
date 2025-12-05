namespace ServiceCommons;

public class OutboxMessageTopicResolver
{
    public static string Resolve(OutboxMessage message)
    {
        if (!string.IsNullOrWhiteSpace(message.Topic))
        {
            return message.Topic.ToLowerInvariant().Trim();
        }

        if (string.IsNullOrWhiteSpace(message.ServiceOriginName))
        {
            throw new ArgumentNullException(nameof(message.ServiceOriginName), "Service name cannot be null or empty.");
        }
        
        if (string.IsNullOrWhiteSpace(message.Type))
        {
            throw new ArgumentNullException(nameof(message.Type), "Event Type name cannot be null or empty.");
        }
        
        return $"{Normalize(message.ServiceOriginName)}.{Normalize(message.Type)}";
    }
    
    private static string Normalize(string input)
        => input.Trim().Replace(" ", "").ToLowerInvariant();
}