using CleanArchitecture.Application.Common.Services.ClientInfo.Models;
using CleanArchitecture.Application.Common.Services.GeoLocation.Models;

namespace CleanArchitecture.Application.Features.Authentication.Queries.GetUserSessions
{
    public sealed class UserSessionDto
    {
        public Guid UserSessionId { get; init; }

        public ClientDeviceInfo Device { get; init; } = null!;

        public GeoLocationInfo Location { get; init; } = null!;

        public DateTimeOffset LastUsedAt { get; init; }
    }
}
