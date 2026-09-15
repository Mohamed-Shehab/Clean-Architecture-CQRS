namespace CleanArchitecture.Application.Features.Authentication.Commands.Login
{
    public sealed class TwoFactorChallengeResponse
    {
        public string PreAuthenticationToken { get; init; } = default!;
    }
}
