using CleanArchitecture.Infrastructure.Authentication;
using CleanArchitecture.Infrastructure.ClientInfo;
using CleanArchitecture.Infrastructure.CurrentUser;
using CleanArchitecture.Infrastructure.Identity;
using CleanArchitecture.Infrastructure.Localization;
using CleanArchitecture.Infrastructure.Persistence;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace CleanArchitecture.Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {

            services
                .AddPersistenceServices(configuration)
                .AddIdentityServices()
                .AddCurrentUserServices()
                .AddClientInfoProviderServices()
                .AddLocalizationServices()
                .AddAuthenticationServices(configuration);


            return services;
        }
    }
}
