namespace CleanArchitecture.Application.Features.Account.Commands.SetupTwoFactor
{
    public sealed class SetupTwoFactorResponse
    {
        public string QrCodeImage { get; init; } = default!;    
    }
}
