namespace CleanArchitecture.Application.Common.Services.Authentication.Models
{
    public sealed class RefreshTokenResult
    {
        public string RefreshToken { get; init; } = default!;

        public DateTime ExpiresAt { get; init; }
    }
}
