namespace CleanArchitecture.Application.Features.Authentication.Models
{
    public sealed class AuthenticationResponse
    {
        public string AccessToken { get; init; } = default!;

        public string TokenType { get; init; } = "Bearer";

        public DateTime AccessTokenExpiresAt { get; init; }

        public string RefreshToken { get; init; } = default!;

        public DateTime RefreshTokenExpiresAt { get; init; }
    }
}
