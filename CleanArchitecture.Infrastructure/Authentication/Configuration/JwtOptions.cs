namespace CleanArchitecture.Infrastructure.Authentication.Configuration
{
    public sealed class JwtOptions
    {
        public const string SectionName = "Jwt";

        public string Issuer { get; init; } = default!;

        public string Audience { get; init; } = default!;

        public string SecretKey { get; init; } = default!;

        public int ExpirationInMinutes { get; init; }
    }
}
