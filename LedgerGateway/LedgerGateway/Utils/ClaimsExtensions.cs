using System.Security.Claims;

namespace LedgerGateway.Application;

public static class ClaimsExtensions
{
    public static string GetEmailOrThrow(this ClaimsPrincipal claim)
    {
        return claim.FindFirstValue(ClaimTypes.Email)
            ?? throw new Exception($"Client email not found");
    }
}