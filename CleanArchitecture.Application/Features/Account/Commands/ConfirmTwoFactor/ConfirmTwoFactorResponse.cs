namespace CleanArchitecture.Application.Features.Account.Commands.ConfirmTwoFactor
{
    public sealed class ConfirmTwoFactorResponse
    {
        public IReadOnlyCollection<string> RecoveryCodes { get; init; }
            = Array.Empty<string>();
    }
}
