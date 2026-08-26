namespace CleanArchitecture.Infrastructure.Authentication.Configurations
{
    public sealed class RefreshTokenOptions
    {
        public const string SectionName = "RefreshToken";

        public int ExpirationInDays { get; init; }
    }
}
