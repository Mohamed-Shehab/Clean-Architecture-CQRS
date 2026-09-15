using CleanArchitecture.Application.Common.Services.QrCode;
using Microsoft.Extensions.DependencyInjection;

namespace CleanArchitecture.Infrastructure.QrCode
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddQrCodeServices(this IServiceCollection services)
        {
            // Registration of QrCode Service
            services.AddSingleton<IQrCodeService, QrCodeService>();


            return services;
        }
    }
}
