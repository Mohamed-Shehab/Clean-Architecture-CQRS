using CleanArchitecture.Application.Common.Services.Localization;
using Microsoft.Extensions.DependencyInjection;

namespace CleanArchitecture.Infrastructure.Localization
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddLocalizationServices(this IServiceCollection services)
        {
            // Registration of Localization Service
            services.AddScoped<ILocalizationService, LocalizationService>();


            return services;
        }
    }
}
