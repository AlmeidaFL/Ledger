using System.Security.Cryptography;
using System.Text;
using Microsoft.IdentityModel.Tokens;

namespace SimpleAuth.Api.Services;

public class TokenGenerator
{
    public static string GenerateRefreshToken(int size = 32)
    {
        var bytes = RandomNumberGenerator.GetBytes(size);
        return Base64UrlEncoder.Encode(bytes);
    }

    public static string GenerateOtp(int size = 48)
    {
        var random = RandomNumberGenerator.GetBytes(size);
        var stringBuilder = new StringBuilder();
        foreach (var @byte in random)
        {
            stringBuilder.Append(@byte % 10);
        }
        return stringBuilder.ToString();
    }
}