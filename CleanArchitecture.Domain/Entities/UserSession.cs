namespace CleanArchitecture.Domain.Entities
{
    public class UserSession
    {
        public Guid UserSessionId { get; set; }

        public int UserId { get; set; }

        // Security / token data
        public string RefreshTokenHash { get; set; } = null!;
        public DateTimeOffset RefreshTokenExpiresAt { get; set; }
        public bool IsExpired => RefreshTokenExpiresAt <= DateTimeOffset.UtcNow;

        public DateTimeOffset? RevokedAt { get; set; }
        public bool IsRevoked => RevokedAt.HasValue;

        // Session lifecycle
        public DateTimeOffset CreatedAt { get; set; }
        public DateTimeOffset? LastUsedAt { get; set; }

        // Request / device information
        public string? IpAddress { get; set; }
        public string? UserAgent { get; set; }
    }
}