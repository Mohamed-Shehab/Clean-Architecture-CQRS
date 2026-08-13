namespace CleanArchitecture.Application.Common.Services.Identity.Enums
{
    public enum ChangeEmailFailureReason
    {
        None = 0,
        UserNotFound,
        InvalidCurrentPassword,
        AccountLocked,
        ChangeEmailFailed
    }
}
