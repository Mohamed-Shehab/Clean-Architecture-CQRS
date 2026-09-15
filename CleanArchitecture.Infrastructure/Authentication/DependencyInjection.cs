using CleanArchitecture.Application.Common.Services.Authentication;
using CleanArchitecture.Infrastructure.Authentication.Configurations;
using CleanArchitecture.Infrastructure.Authentication.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace CleanArchitecture.Infrastructure.Authentication
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddAuthenticationServices(this IServiceCollection services, IConfiguration configuration)
        {
            // Register Jwt Options configuration and validation
            services.ConfigureOptions<JwtOptionsSetup>();

            services
                .AddOptions<JwtOptions>()
                .ValidateOnStart();


            // Register JwtTokenService
            services.AddScoped<IJwtTokenService, JwtTokenService>();


            // Configure JWT authentication
            services
                .AddAuthentication(options =>
                {
                    options.DefaultAuthenticateScheme =
                        JwtBearerDefaults.AuthenticationScheme;

                    options.DefaultChallengeScheme =
                        JwtBearerDefaults.AuthenticationScheme;
                })
                .AddJwtBearer();

            services
                .AddOptions<JwtBearerOptions>(JwtBearerDefaults.AuthenticationScheme)
                .Configure<IOptions<JwtOptions>>((options, jwtOptionsProvider) =>
                {
                    var jwtOptions = jwtOptionsProvider.Value;

                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = true,
                        ValidIssuer = jwtOptions.Issuer,

                        ValidateAudience = true,
                        ValidAudience = jwtOptions.Audience,

                        ValidateIssuerSigningKey = true,
                        IssuerSigningKey = new SymmetricSecurityKey(
                            Encoding.UTF8.GetBytes(jwtOptions.SecretKey)),

                        RequireExpirationTime = true,
                        ValidateLifetime = true,

                        ClockSkew = TimeSpan.Zero
                    };
                });


            // Register Authorization Policies
            services
                .AddAuthorization(options =>
                {
                    // Configure Default Policy
                    options.DefaultPolicy = new AuthorizationPolicyBuilder()
                        .RequireAuthenticatedUser()
                        .RequireClaim("purpose", "access")
                        .Build();

                    // Configure Two-Factor Authentication Policy
                    options.AddPolicy("TwoFactorPreAuth", policy =>
                    {
                        policy.RequireAuthenticatedUser();
                        policy.RequireClaim("purpose", "2fa");
                    });
                });


            // Register RefreshToken Options Configuration and Validation
            services.ConfigureOptions<RefreshTokenOptionsSetup>();

            services
                .AddOptions<RefreshTokenOptions>()
                .ValidateOnStart();


            // Register RefreshTokenService
            services.AddScoped<IRefreshTokenService, RefreshTokenService>();


            return services;
        }
    }
}
