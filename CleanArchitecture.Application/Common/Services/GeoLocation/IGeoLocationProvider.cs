using CleanArchitecture.Application.Common.Services.GeoLocation.Models;

namespace CleanArchitecture.Application.Common.Services.GeoLocation
{
    public interface IGeoLocationProvider
    {
        GeoLocationInfo GetLocation(string? ipAddress);
    }
}
