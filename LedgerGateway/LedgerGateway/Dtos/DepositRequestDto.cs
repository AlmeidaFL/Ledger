using System.Text.Json.Serialization;

namespace LedgerGateway.Dtos;

public class DepositRequestDto
{
    [JsonIgnore]
    public string? UserEmail { get; set; }
    public long Amount { get; set; }
    public string Currency { get; set; }
    public string IdempotencyKey { get; set; }
}