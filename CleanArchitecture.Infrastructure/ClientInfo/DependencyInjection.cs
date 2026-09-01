using CleanArchitecture.Application.Common.Services.ClientInfo;
using Microsoft.Extensions.DependencyInjection;
using UAParser;

namespace CleanArchitecture.Infrastructure.ClientInfo
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddClientInfoServices(this IServiceCollection services)
        {
            services.AddScoped<IClientInfoProvider, ClientInfoProvider>();

            services.AddSingleton(_ => Parser.GetDefault());
            services.AddSingleton<IUserAgentParser, UserAgentParser>();


            return services;
        }
    }
}
