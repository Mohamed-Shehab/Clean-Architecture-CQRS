using CleanArchitecture.Application.Features.Authentication.Models;

namespace CleanArchitecture.Application.Features.Authentication.Commands.Login
{
    public sealed class LoginResponse
    {
        public bool RequiresTwoFactor { get; init; }

        public TokenResponse? Tokens { get; init; }

        public TwoFactorChallengeResponse? TwoFactor { get; init; }
    }
}
