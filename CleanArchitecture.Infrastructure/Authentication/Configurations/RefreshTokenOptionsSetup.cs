using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;

namespace CleanArchitecture.Infrastructure.Authentication.Configurations
{
    public sealed class RefreshTokenOptionsSetup :
        IConfigureOptions<RefreshTokenOptions>,
        IValidateOptions<RefreshTokenOptions>

    {
        private readonly IConfiguration _configuration;


        public RefreshTokenOptionsSetup(IConfiguration configuration)
        {
            this._configuration = configuration;
        }


        public void Configure(RefreshTokenOptions options)
        {
            _configuration
                .GetSection(RefreshTokenOptions.SectionName)
                .Bind(options);
        }


        public ValidateOptionsResult Validate(string? name, RefreshTokenOptions options)
        {
            if (options.ExpirationInDays <= 0)
                return ValidateOptionsResult.Fail("RefreshToken:ExpirationInDays must be greater than zero.");

            return ValidateOptionsResult.Success;
        }
    }
}
