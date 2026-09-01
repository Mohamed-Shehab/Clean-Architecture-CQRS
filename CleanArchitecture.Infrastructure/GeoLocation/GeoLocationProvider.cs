using CleanArchitecture.Application.Common.Services.GeoLocation;
using CleanArchitecture.Application.Common.Services.GeoLocation.Models;
using MaxMind.GeoIP2;
using MaxMind.GeoIP2.Exceptions;
using MaxMind.GeoIP2.Responses;
using System.Net;

namespace CleanArchitecture.Infrastructure.GeoLocation
{
    public sealed class GeoLocationProvider : IGeoLocationProvider
    {
        private readonly DatabaseReader _databaseReader;


        public GeoLocationProvider(DatabaseReader databaseReader)
        {
            this._databaseReader = databaseReader;
        }


        public GeoLocationInfo GetLocation(string? ipAddress)
        {
            if (string.IsNullOrWhiteSpace(ipAddress))
            {
                return new GeoLocationInfo();
            }


            if (!IPAddress.TryParse(ipAddress, out var ip))
            {
                return new GeoLocationInfo();
            }


            CityResponse response;

            try
            {
                response = _databaseReader.City(ip);
            }
            catch (AddressNotFoundException)
            {
                return new GeoLocationInfo();
            }


            return new GeoLocationInfo
            {
                Country = FormatLocationName(response.Country?.Name, response.Country?.IsoCode),

                Region = FormatLocationName(response.MostSpecificSubdivision?.Name, response.MostSpecificSubdivision?.IsoCode),

                City = response.City?.Name
            };
        }


        private static string? FormatLocationName(string? name, string? isoCode)
        {
            if (string.IsNullOrWhiteSpace(name))
                return isoCode;

            return string.IsNullOrWhiteSpace(isoCode)
                ? name
                : $"{name} ({isoCode})";
        }
    }
}
