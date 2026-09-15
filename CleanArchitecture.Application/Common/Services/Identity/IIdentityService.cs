using CleanArchitecture.Application.Common.Services.Authentication.Models;
using CleanArchitecture.Application.Common.Services.Identity.Models;
using System;

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


        Task<AuthenticatedUser?> CompleteLoginAsync(
            string userId);


        Task<AuthenticatedUser?> GetAuthenticatedUserAsync(
            string userId);


        Task<string?> GenerateAuthenticatorKeyAsync(
            string userId);


        Task<TwoFactorVerificationResult> VerifyTwoFactorCodeAsync(
            string userId,
            string code);


        Task<bool> DisableTwoFactorAuthenticationAsync(
            string userId);


        Task<IReadOnlyCollection<string>?> GenerateTwoFactorRecoveryCodesAsync(
            string userId,
            int numberOfCodes);


        Task<TwoFactorConfirmationResult> ConfirmTwoFactorAsync(
            string userId,
            string code,
            int numberOfRecoveryCodes);


        Task<bool> RedeemTwoFactorRecoveryCodeAsync(
            string userId,
            string recoveryCode);
    }
}
