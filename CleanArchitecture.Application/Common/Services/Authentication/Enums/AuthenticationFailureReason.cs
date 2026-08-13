namespace CleanArchitecture.Application.Common.Services.Authentication.Enums
{
    public enum AuthenticationFailureReason
    {
        None,

        InvalidCredentials,

        EmailNotConfirmed,

        AccountLocked
    }
}
