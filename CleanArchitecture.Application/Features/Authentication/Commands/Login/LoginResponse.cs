namespace CleanArchitecture.Application.Features.Authentication.Commands.Login
{
    public sealed class LoginResponse
    {
        public string AccessToken { get; init; } = default!;

        public string TokenType { get; init; } = "Bearer";

        public DateTime AccessTokenExpiresAt { get; init; }

        public string RefreshToken { get; init; } = default!;
    }
}
