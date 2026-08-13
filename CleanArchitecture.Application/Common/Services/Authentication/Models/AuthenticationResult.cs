using CleanArchitecture.Application.Common.Services.Authentication.Enums;

namespace CleanArchitecture.Application.Common.Services.Authentication.Models
{
    public sealed class AuthenticationResult
    {
        public bool Succeeded { get; init; }

        public AuthenticatedUser? User { get; init; }

        public AuthenticationFailureReason FailureReason { get; init; }
    }
}
