using CleanArchitecture.Application.Common.Services.CurrentUser;
using Microsoft.Extensions.DependencyInjection;

namespace CleanArchitecture.Infrastructure.CurrentUser
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddCurrentUserServices(this IServiceCollection services)
        {
            services.AddHttpContextAccessor();

            services.AddScoped<ICurrentUserService, CurrentUserService>();


            return services;
        }
    }
}
