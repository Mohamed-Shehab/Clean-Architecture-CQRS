using CleanArchitecture.Application.Common.Services.ClientInfo.Models;

namespace CleanArchitecture.Application.Common.Services.ClientInfo
{
    public interface IUserAgentParser
    {
        ClientDeviceInfo Parse(string? userAgent);
    }
}
