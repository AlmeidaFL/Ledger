using LedgerGateway.Dtos;

namespace LedgerGateway.Application;

public interface IUserAgentParser
{
    UserAgentInfoDto Parse(string userAgent);
}

public class UserAgentParser : IUserAgentParser
{
    public UserAgentInfoDto Parse(string agent)
    {
        var info = new UserAgentInfoDto
        {
            UserAgent = agent
        };

        if (string.IsNullOrWhiteSpace(agent))
            return info;
        
        if (agent.Contains("Chrome"))
        {
            info.BrowserFamily = "Chrome";
            info.BrowserVersion = ExtractVersion(agent, "Chrome/");
        }
        else if (agent.Contains("Safari") && agent.Contains("Version/"))
        {
            info.BrowserFamily = "Safari";
            info.BrowserVersion = ExtractVersion(agent, "Version/");
        }
        else if (agent.Contains("Firefox"))
        {
            info.BrowserFamily = "Firefox";
            info.BrowserVersion = ExtractVersion(agent, "Firefox/");
        }
        else if (agent.Contains("Edg"))
        {
            info.BrowserFamily = "Edge";
            info.BrowserVersion = ExtractVersion(agent, "Edg/");
        }
        
        if (agent.Contains("Windows"))
            info.DeviceFamily = "Windows";
        else if (agent.Contains("Macintosh"))
            info.DeviceFamily = "macOS";
        else if (agent.Contains("iPhone"))
            info.DeviceFamily = "iPhone";
        else if (agent.Contains("Android"))
            info.DeviceFamily = "Android";

        // Brand
        if (info.DeviceFamily == "iPhone" || info.DeviceFamily == "macOS")
            info.DeviceBrand = "Apple";

        // Friendly name
        info.ClientName = $"{info.BrowserFamily} on {info.DeviceFamily}".Trim();

        return info;
    }

    private string ExtractVersion(string agent, string prefix)
    {
        var start = agent.IndexOf(prefix);
        if (start < 0) return "";

        start += prefix.Length;
        var end = agent.IndexOf(" ", start);
        if (end < 0) end = agent.Length;

        return agent.Substring(start, end - start);
    }
}
