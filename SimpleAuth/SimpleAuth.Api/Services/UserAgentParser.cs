using SimpleAuth.Api.Data;

namespace SimpleAuth.Api.Services;

public class UserAgentInfo
{
    public string IpAddress { get; set; } = string.Empty;
    public string UserAgent { get; set; } = string.Empty;
    public string BrowserFamily { get; set; } = "";
    public string BrowserVersion { get; set; } = "";
    public string DeviceFamily { get; set; } = "";
    public string DeviceBrand { get; set; } = "";
    public string ClientName { get; set; } = "";
    public string ClientType { get; set; } = "Web";
}

public interface IUserAgentParser
{
    UserAgentInfo Parse(string userAgent);
}

public class UserAgentParser : IUserAgentParser
{
    public UserAgentInfo Parse(string agent)
    {
        var info = new UserAgentInfo();
        info.UserAgent = agent;

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
