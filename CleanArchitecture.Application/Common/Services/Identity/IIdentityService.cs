using CleanArchitecture.Application.Common.Services.Authentication.Models;
using CleanArchitecture.Application.Common.Services.Identity.Models;

namespace CleanArchitecture.Application.Common.Services.Identity
{
    public interface IIdentityService
    {
        Task<bool> IsEmailUsedAsync(string email, CancellationToken cancellationToken);


        Task<bool> IsPhoneNumberUsedAsync(string phoneNumber, CancellationToken cancellationToken);


        Task<CreateUserResult> CreateUserAsync(
            string firstName,
            string lastName,
            string email,
            string phoneNumber,
            string password);


        Task<bool> IsPhoneUsedByAnotherUserAsync(
            int userId,
            string phoneNumber,
            CancellationToken cancellationToken);


        Task<IdentityOperationResult> UpdateUserAsync(int userId, 
            string firstName, 
            string lastName, 
            string phoneNumber, 
            CancellationToken cancellationToken);


        Task<IdentityOperationResult> DeleteUserAsync(int userId, CancellationToken cancellationToken);


        Task<IdentityOperationResult> ChangePasswordAsync(
            int userId,
            string currentPassword,
            string newPassword,
            CancellationToken cancellationToken = default);


        Task<ChangeEmailResult> ChangeEmailAsync(
            int userId,
            string currentPassword,
            string newEmail,
            CancellationToken cancellationToken = default);


        Task<AuthenticationResult> AuthenticateAsync(
            string email,
            string password,
            CancellationToken cancellationToken = default);


        Task<AuthenticatedUser?> GetUserByIdAsync(
            int userId,
            CancellationToken cancellationToken = default);
    }
}
