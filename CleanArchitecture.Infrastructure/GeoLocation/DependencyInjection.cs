using CleanArchitecture.Application.Common.Services.GeoLocation;
using MaxMind.Db;
using MaxMind.GeoIP2;
using Microsoft.Extensions.DependencyInjection;

namespace CleanArchitecture.Infrastructure.GeoLocation
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddGeoLocationServices(this IServiceCollection services)
        {
            services.AddSingleton<DatabaseReader>(serviceProvider =>
            {
                var databasePath = Path.Combine(
                    AppContext.BaseDirectory,
                    "GeoLocation",
                    "Database",
                    "GeoLite2-City.mmdb");

                return new DatabaseReader(databasePath, mode: FileAccessMode.MemoryMapped);
            });

            services.AddSingleton<IGeoLocationProvider, GeoLocationProvider>();


            return services;
        }
    }
}
