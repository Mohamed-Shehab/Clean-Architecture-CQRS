using CleanArchitecture.Application.Common.Services.ClientInfo;
using CleanArchitecture.Application.Common.Services.ClientInfo.Models;
using UAParser;
using UAParser.Objects;

namespace CleanArchitecture.Infrastructure.ClientInfo
{
    public sealed class UserAgentParser : IUserAgentParser
    {
        private readonly Parser _parser;


        public UserAgentParser(Parser parser)
        {
            this._parser = parser;
        }


        public ClientDeviceInfo Parse(string? userAgent)
        {
            if (string.IsNullOrWhiteSpace(userAgent))
            {
                return new ClientDeviceInfo
                {
                    DeviceType = "Unknown",
                    OperatingSystem = "Unknown",
                    Browser = "Unknown"
                };
            }

            var clientInfo = _parser.Parse(userAgent);

            return new ClientDeviceInfo
            {
                DeviceType = GetDeviceType(clientInfo.Device),
                OperatingSystem = GetOperatingSystem(clientInfo.OS),
                Browser = GetBrowser(clientInfo.Browser)
            };
        }


        private static string GetDeviceType(Device device)
        {
            var deviceFamily = device.Family;

            if (string.IsNullOrWhiteSpace(deviceFamily) ||
                string.Equals(deviceFamily, "Other", StringComparison.OrdinalIgnoreCase))
            {
                return "Unknown";
            }

            if (deviceFamily.Contains("iPad", StringComparison.OrdinalIgnoreCase) ||
                deviceFamily.Contains("Tablet", StringComparison.OrdinalIgnoreCase))
            {
                return "Tablet";
            }

            if (deviceFamily.Contains("iPhone", StringComparison.OrdinalIgnoreCase) ||
                deviceFamily.Contains("Mobile", StringComparison.OrdinalIgnoreCase))
            {
                return "Mobile";
            }

            return "Desktop";
        }


        private static string GetOperatingSystem(OS os)
        {
            if (string.IsNullOrWhiteSpace(os.Family) ||
                string.Equals(os.Family, "Other", StringComparison.OrdinalIgnoreCase))
            {
                return "Unknown";
            }

            return string.IsNullOrWhiteSpace(os.Major) ? os.Family : $"{os.Family} {os.Major}";
        }


        private static string GetBrowser(Browser browser)
        {
            if (string.IsNullOrWhiteSpace(browser.Family) ||
                string.Equals(browser.Family, "Other", StringComparison.OrdinalIgnoreCase))
            {
                return "Unknown";
            }

            return string.IsNullOrWhiteSpace(browser.Major) ? browser.Family : $"{browser.Family} {browser.Major}";
        }
    }
}
