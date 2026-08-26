namespace CleanArchitecture.Application.Common.Services.ClientInfo
{
    public interface IClientInfoProvider
    {
        string? IpAddress { get; }

        string? UserAgent { get; }
    }
}
