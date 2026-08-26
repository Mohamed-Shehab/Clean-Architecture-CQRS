using CleanArchitecture.Application.Common.Services.ClientInfo;
using Microsoft.Extensions.DependencyInjection;

namespace CleanArchitecture.Infrastructure.ClientInfo
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddClientInfoProviderServices(this IServiceCollection services)
        {
            services.AddScoped<IClientInfoProvider, ClientInfoProvider>();

            return services;
        }
    }
}
