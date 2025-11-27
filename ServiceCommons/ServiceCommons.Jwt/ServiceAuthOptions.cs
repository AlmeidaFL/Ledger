namespace ServiceCommons.Jwt;

public class ServiceAuthOptions
{
    public const string SectionName = "ServiceAuth";
    public List<string> AllowedServices { get; init; } = [];
}