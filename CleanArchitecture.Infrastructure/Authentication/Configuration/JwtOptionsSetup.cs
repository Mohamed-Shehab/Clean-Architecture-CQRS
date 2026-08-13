using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;

namespace CleanArchitecture.Infrastructure.Authentication.Configuration
{
    public sealed class JwtOptionsSetup :
        IConfigureOptions<JwtOptions>,
        IValidateOptions<JwtOptions>
    {
        private readonly IConfiguration _configuration;


        public JwtOptionsSetup(IConfiguration configuration)
        {
            _configuration = configuration;
        }


        public void Configure(JwtOptions options)
        {
            _configuration
                .GetSection(JwtOptions.SectionName)
                .Bind(options);
        }


        public ValidateOptionsResult Validate(string? name, JwtOptions options)
        {
            if (string.IsNullOrWhiteSpace(options.Issuer))
                return ValidateOptionsResult.Fail("Jwt:Issuer is required.");


            if (string.IsNullOrWhiteSpace(options.Audience))
                return ValidateOptionsResult.Fail("Jwt:Audience is required.");


            if (string.IsNullOrWhiteSpace(options.SecretKey))
                return ValidateOptionsResult.Fail("Jwt__SecretKey is missing.");


            if (options.SecretKey.Length < 32)
                return ValidateOptionsResult.Fail("Jwt secret key must be at least 32 characters.");


            if (options.ExpirationInMinutes <= 0)
                return ValidateOptionsResult.Fail("Expiration must be greater than zero.");


            return ValidateOptionsResult.Success;
        }
    }
}
