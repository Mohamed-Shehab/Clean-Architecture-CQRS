namespace CleanArchitecture.Application.Common.Services.ClientInfo.Models
{
    public sealed class ClientDeviceInfo
    {
        public string? DeviceType { get; init; }

        public string? OperatingSystem { get; init; }

        public string? Browser { get; init; }
    }
}
