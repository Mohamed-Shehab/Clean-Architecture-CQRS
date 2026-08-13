using CleanArchitecture.Application.Common.Services.Identity.Enums;

namespace CleanArchitecture.Application.Common.Services.Identity.Models
{
    public class ChangeEmailResult : IdentityOperationResult
    {
        public ChangeEmailFailureReason FailureReason { get; set; } = ChangeEmailFailureReason.None;
    }
}
