using CleanArchitecture.Application.Behaviors;
using CleanArchitecture.Application.Common.Services.Authentication;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.DependencyInjection;

namespace CleanArchitecture.Application
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddApplication(this IServiceCollection services)
        {
            // Registration of MediatR
            services.AddMediatR(cfg =>
            {
                cfg.RegisterServicesFromAssembly(typeof(DependencyInjection).Assembly);
            });

            // Registration of Auto Mapper
            services.AddAutoMapper(typeof(DependencyInjection).Assembly);

            // Registration of Fluent Validation
            services.AddValidatorsFromAssembly(typeof(DependencyInjection).Assembly);

            // Registration of Behavior
            services.AddTransient(
                typeof(IPipelineBehavior<,>),
                typeof(ValidationBehavior<,>)
            );

            // Registration of Authentication Completion flow
            services.AddScoped<IAuthenticationCompletionService, AuthenticationCompletionService>();

            return services;
        }
    }
}
