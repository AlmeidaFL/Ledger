using System.Text;

namespace ServiceCommons.ApiKey;

public class InternalApiSettings
{
    public const string SectionName = "InternalApi";
    
    public string ApiKey { get; set; }
    
    private byte[]? cached;
    public ReadOnlySpan<byte> ApiKeyBytes 
        => cached ??= Encoding.UTF8.GetBytes(ApiKey);
}