using System.ComponentModel.DataAnnotations;

namespace LedgerGateway.Dtos;

public class RefreshRequestDto
{
    [Required]
    public string Token { get; set; }
}