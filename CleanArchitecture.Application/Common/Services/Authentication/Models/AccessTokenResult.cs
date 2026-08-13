namespace CleanArchitecture.Application.Common.Services.Authentication.Models
{
    public sealed class AccessTokenResult
    {
        public string AccessToken { get; init; } = default!;

        public DateTime ExpiresAt { get; init; }
    }
}
