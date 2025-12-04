namespace LedgerGateway.Dtos;

public class UserAgentInfoDto
{
    public string IpAddress { get; set; } = string.Empty;
    public string UserAgent { get; set; } = string.Empty;
    public string BrowserFamily { get; set; } = string.Empty;
    public string BrowserVersion { get; set; } = string.Empty;
    public string DeviceFamily { get; set; } = string.Empty;
    public string DeviceBrand { get; set; } = string.Empty;
    public string ClientName { get; set; } = string.Empty;
    public string ClientType { get; set; } = string.Empty;
}
