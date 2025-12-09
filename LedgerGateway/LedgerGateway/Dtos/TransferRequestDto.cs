using System.Text.Json.Serialization;

namespace LedgerGateway.Dtos;

public class TransferRequestDto
{
    [JsonIgnore]
    public string FromAccountEmail { get; set; }
    public string ToAccountEmail { get; set; }
    public long Amount { get; set; }
    public string Currency { get; set; }
    public string IdempotencyKey { get; set; }
    public string? Metadata { get; set; }
}