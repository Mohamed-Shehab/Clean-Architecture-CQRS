namespace CleanArchitecture.Application.Features.Account.Commands.RegenerateTwoFactorRecoveryCodes
{
    public sealed class RegenerateTwoFactorRecoveryCodesResponse
    {
        public IReadOnlyCollection<string> RecoveryCodes { get; init; } = Array.Empty<string>();
    }
}
