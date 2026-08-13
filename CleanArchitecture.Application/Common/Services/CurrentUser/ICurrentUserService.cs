namespace CleanArchitecture.Application.Common.Services.CurrentUser
{
    public interface ICurrentUserService
    {
        int UserId { get; }

        string Email { get; }

        bool IsAuthenticated { get; }
    }
}
