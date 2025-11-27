namespace ServiceCommons.Jwt;

public class JwtOptions
{
    public const string SectionName = "Jwt";
    
    public string Issuer { get; set; }
    public string Audience { get; set; }
    public string Secret { get; init; }
    public int AccessTokenExpiration { get; set; } = 30;
    public int ClockSkewMinutes { get; set; } = 2;
}