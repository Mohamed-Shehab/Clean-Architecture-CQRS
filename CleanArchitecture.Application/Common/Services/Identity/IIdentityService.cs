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

        Task<IdentityOperationResult> UpdateUserAsync(int userId, 
            string firstName, 
            string lastName, 
            string phoneNumber, 
            CancellationToken cancellationToken);

        Task<IdentityOperationResult> DeleteUserAsync(int userId, CancellationToken cancellationToken);

    }
}
