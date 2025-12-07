using System.ComponentModel.DataAnnotations;

namespace LedgerGateway.Dtos;

public class RefreshRequestDto
{
    public string? Token { get; set; }
}