namespace CleanArchitecture.Application.Common.Services.Identity.Models
{
    public sealed class TwoFactorVerificationResult
    {
        public bool Succeeded { get; init; }

        public DateTimeOffset? LockedUntil { get; init; }

        public int? RemainingAttempts { get; init; }
    }
}
