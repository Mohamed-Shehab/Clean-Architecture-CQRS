namespace CleanArchitecture.Application.Common.Services.Identity.Models
{
    public sealed class TwoFactorConfirmationResult
    {
        public bool Succeeded { get; init; }

        public DateTimeOffset? LockedUntil { get; init; }

        public int? RemainingAttempts { get; init; }

        public IReadOnlyCollection<string>? RecoveryCodes { get; init; }
    }
}
