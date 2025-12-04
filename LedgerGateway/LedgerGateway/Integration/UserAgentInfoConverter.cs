using LedgerGateway.Dtos;
using LedgerGateway.RestClients.SimpleAuth;

namespace LedgerGateway.Integration;

public static class UserAgentInfoConverter
{
    public static UserAgentInfo ToUserAgentInfo(this UserAgentInfoDto dto)
    {
        return new UserAgentInfo
        {
            BrowserFamily = dto.BrowserFamily,
            BrowserVersion = dto.BrowserVersion,
            DeviceFamily = dto.DeviceFamily,
            ClientName = dto.ClientName,
            ClientType = dto.ClientType,
            DeviceBrand = dto.DeviceBrand,
            IpAddress = dto.IpAddress,
            UserAgent = dto.UserAgent,
        };
    }
}