using CleanArchitecture.Application.Common.Localization;
using CleanArchitecture.Application.Common.Localization.Resources;
using CleanArchitecture.Application.Common.Services.Authentication.Enums;
using CleanArchitecture.Application.Common.Services.Authentication.Models;
using CleanArchitecture.Application.Common.Services.Identity;
using CleanArchitecture.Application.Common.Services.Identity.Enums;
using CleanArchitecture.Application.Common.Services.Identity.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;
using System.Data;

namespace CleanArchitecture.Infrastructure.Identity.Services
{
    public class IdentityService : IIdentityService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly IStringLocalizer<SharedResources> _localizer;


        public IdentityService(UserManager<ApplicationUser> userManager,
                               SignInManager<ApplicationUser> signInManager,
                               IStringLocalizer<SharedResources> localizer)
        {
            this._userManager = userManager;
            this._signInManager = signInManager;
            this._localizer = localizer;
        }


        public Task<bool> IsEmailUsedAsync(string email,
                                           CancellationToken cancellationToken)
        {
            return _userManager.Users.AnyAsync(u => u.Email == email, cancellationToken);
        }


        public Task<bool> IsPhoneNumberUsedAsync(string phoneNumber,
                                                 CancellationToken cancellationToken)
        {
            return _userManager.Users.AnyAsync(u => u.PhoneNumber == phoneNumber, cancellationToken);
        }


        public async Task<CreateUserResult> CreateUserAsync(string firstName,
                                                            string lastName,
                                                            string email,
                                                            string phoneNumber,
                                                            string password)
        {
            var user = new ApplicationUser()
            {
                FirstName = firstName,
                LastName = lastName,
                Email = email,
                PhoneNumber = phoneNumber,
                UserName = email,
                CreatedAt = DateTime.UtcNow
            };

            var result = await _userManager.CreateAsync(user, password);

            if (!result.Succeeded)
            {
                return new CreateUserResult
                {
                    Succeeded = false,
                    Errors = result.Errors.Select(e => e.Description).ToList()
                };
            }

            return new CreateUserResult
            {
                Succeeded = true,
                UserId = user.Id,
                Errors = []
            };
        }


        public async Task<bool> IsPhoneUsedByAnotherUserAsync(int userId,
                                                              string phoneNumber,
                                                              CancellationToken cancellationToken)
        {
            return await _userManager.Users
                .AnyAsync(u => u.Id != userId && u.PhoneNumber == phoneNumber, cancellationToken);
        }


        public async Task<IdentityOperationResult> UpdateUserAsync(int userId,
                                                                   string firstName,
                                                                   string lastName,
                                                                   string phoneNumber,
                                                                   CancellationToken cancellationToken)
        {
            var user = await _userManager.Users.FirstOrDefaultAsync(u => u.Id == userId, cancellationToken);

            if (user is null)
            {
                return new IdentityOperationResult()
                {
                    Succeeded = false,
                    Errors = [_localizer[Messages.NotFound, _localizer[Entities.User]]]
                };
            }

            user.FirstName = firstName;
            user.LastName = lastName;
            user.PhoneNumber = phoneNumber;
            user.UpdatedAt = DateTime.UtcNow;

            var result = await _userManager.UpdateAsync(user);

            if (!result.Succeeded)
            {
                return new IdentityOperationResult
                {
                    Succeeded = false,
                    Errors = result.Errors.Select(e => e.Description).ToList()
                };
            }

            return new IdentityOperationResult
            {
                Succeeded = true,
                Errors = []
            };
        }


        public async Task<IdentityOperationResult> DeleteUserAsync(int userId,
                                                                   CancellationToken cancellationToken)
        {
            var user = await _userManager.Users.FirstOrDefaultAsync(u => u.Id == userId, cancellationToken);

            if (user == null)
            {
                return new IdentityOperationResult()
                {
                    Succeeded = false,
                    Errors = new List<string>()
                };
            }

            if (user.IsDeleted)
            {
                return new IdentityOperationResult
                {
                    Succeeded = false,
                    Errors = new List<string>()
                };
            }

            user.IsDeleted = true;
            user.DeletedAt = DateTime.UtcNow;
            user.IsActive = false;

            var result = await _userManager.UpdateAsync(user);

            if (!result.Succeeded)
            {
                return new IdentityOperationResult
                {
                    Succeeded = false,
                    Errors = result.Errors.Select(e => e.Description).ToList()
                };
            }

            return new IdentityOperationResult
            {
                Succeeded = true,
                Errors = new List<string>()
            };
        }


        public async Task<IdentityOperationResult> ChangePasswordAsync(int userId,
                                                                       string currentPassword,
                                                                       string newPassword,
                                                                       CancellationToken cancellationToken = default)
        {
            var user = await _userManager.FindByIdAsync(userId.ToString());

            if (user is null)
            {
                return new IdentityOperationResult
                {
                    Succeeded = false,
                    Errors = [_localizer[Messages.NotFound, _localizer[Entities.User]]]
                };
            }


            var result = await _userManager.ChangePasswordAsync(user, currentPassword, newPassword);

            if (!result.Succeeded)
            {
                return new IdentityOperationResult
                {
                    Succeeded = false,
                    Errors = result.Errors.Select(e => e.Description).ToList()
                };
            }


            return new IdentityOperationResult
            {
                Succeeded = true,
                Errors = []
            };
        }


        public async Task<ChangeEmailResult> ChangeEmailAsync(int userId,
                                                              string currentPassword,
                                                              string newEmail,
                                                              CancellationToken cancellationToken = default)
        {
            var user = await _userManager.FindByIdAsync(userId.ToString());

            if (user is null)
            {
                return new ChangeEmailResult
                {
                    Succeeded = false,
                    FailureReason = ChangeEmailFailureReason.UserNotFound
                };
            }

            // Verify the current password before allowing the email change.
            var passwordResult = await _signInManager.CheckPasswordSignInAsync(
                user,
                currentPassword,
                lockoutOnFailure: true);


            if (passwordResult.IsLockedOut)
            {
                return new ChangeEmailResult
                {
                    Succeeded = false,
                    FailureReason = ChangeEmailFailureReason.AccountLocked
                };
            }

            if (!passwordResult.Succeeded)
            {
                return new ChangeEmailResult
                {
                    Succeeded = false,
                    FailureReason = ChangeEmailFailureReason.InvalidCurrentPassword
                };
            }

            // Update the email.
            var emailResult = await _userManager.SetEmailAsync(user, newEmail);

            if (!emailResult.Succeeded)
            {
                return new ChangeEmailResult
                {
                    Succeeded = false,
                    Errors = emailResult.Errors
                        .Select(e => e.Description).ToList(),

                    FailureReason = ChangeEmailFailureReason.ChangeEmailFailed
                };
            }


            // Keep the Identity username synchronized with the email.
            var usernameResult = await _userManager.SetUserNameAsync(
                user,
                newEmail);


            if (!usernameResult.Succeeded)
            {
                return new ChangeEmailResult
                {
                    Succeeded = false,
                    Errors = usernameResult.Errors
                        .Select(e => e.Description).ToList(),

                    FailureReason = ChangeEmailFailureReason.ChangeEmailFailed
                };
            }


            return new ChangeEmailResult
            {
                Succeeded = true,
                Errors = [],
                FailureReason = ChangeEmailFailureReason.None
            };
        }


        public async Task<AuthenticationResult> AuthenticateAsync(string email,
                                                                  string password,
                                                                  CancellationToken cancellationToken = default)
        {
            var user = await _userManager.FindByEmailAsync(email);

            if (user is null)
            {
                return new AuthenticationResult
                {
                    Succeeded = false,
                    FailureReason = AuthenticationFailureReason.InvalidCredentials
                };
            }


            if (!await _userManager.IsEmailConfirmedAsync(user))
            {
                return new AuthenticationResult
                {
                    Succeeded = false,
                    FailureReason = AuthenticationFailureReason.EmailNotConfirmed
                };
            }



            var passwordResult = await _signInManager.CheckPasswordSignInAsync(
                user,
                password,
                lockoutOnFailure: true);

            if (passwordResult.IsLockedOut)
            {
                return new AuthenticationResult
                {
                    Succeeded = false,
                    FailureReason = AuthenticationFailureReason.AccountLocked,
                    LockedUntil = await _userManager.GetLockoutEndDateAsync(user),
                    RemainingAttempts = 0
                };
            }


            if (!passwordResult.Succeeded)
            {
                var failedCount = await _userManager.GetAccessFailedCountAsync(user);

                var maxAttempts = _userManager.Options.Lockout.MaxFailedAccessAttempts;

                var remainingAttempts = Math.Max(0, maxAttempts - failedCount);


                return new AuthenticationResult
                {
                    Succeeded = false,
                    FailureReason = AuthenticationFailureReason.InvalidCredentials,
                    RemainingAttempts = remainingAttempts,
                    LockedUntil = await _userManager.GetLockoutEndDateAsync(user)
                };
            }


            var twoFactorEnabled = await _userManager.GetTwoFactorEnabledAsync(user);

            return new AuthenticationResult
            {
                Succeeded = true,

                RequiresTwoFactor = twoFactorEnabled,

                User = twoFactorEnabled
                    ? new AuthenticatedUser { Id = user.Id }
                    : await CompleteLoginAsync(user)
            };
        }


        private async Task<AuthenticatedUser?> CompleteLoginAsync(ApplicationUser? user)
        {
            if (user == null)
            {
                return null;
            }


            user.LastLoginAt = DateTime.UtcNow;
            await _userManager.UpdateAsync(user);


            return await GetAuthenticatedUserAsync(user);
        }


        public async Task<AuthenticatedUser?> CompleteLoginAsync(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);

            return await CompleteLoginAsync(user);
        }


        private async Task<AuthenticatedUser?> GetAuthenticatedUserAsync(ApplicationUser? user)
        {
            if (user == null)
            {
                return null;
            }


            var roles = await _userManager.GetRolesAsync(user);

            return new AuthenticatedUser
            {
                Id = user.Id,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Email = user.Email!,
                Roles = roles.ToArray(),
                Permissions = Array.Empty<string>() //todo: Permissions are not implemented yet
            };
        }


        public async Task<AuthenticatedUser?> GetAuthenticatedUserAsync(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);

            return await GetAuthenticatedUserAsync(user);
        }


        public async Task<string?> GenerateAuthenticatorKeyAsync(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);

            if (user is null)
            {
                return null;
            }

            await _userManager.ResetAuthenticatorKeyAsync(user);

            return await _userManager.GetAuthenticatorKeyAsync(user);
        }


        public async Task<TwoFactorVerificationResult> VerifyTwoFactorCodeAsync(string userId,
                                                                                string code)
        {
            var user = await _userManager.FindByIdAsync(userId);

            if (user is null)
            {
                return new TwoFactorVerificationResult
                {
                    Succeeded = false
                };
            }

            return await VerifyTwoFactorCodeAsync(user, code);
        }


        private async Task<TwoFactorVerificationResult> VerifyTwoFactorCodeAsync(ApplicationUser user,
                                                                                 string code)
        {
            if (await _userManager.IsLockedOutAsync(user))
            {
                return new TwoFactorVerificationResult
                {
                    Succeeded = false,
                    LockedUntil = await _userManager.GetLockoutEndDateAsync(user)
                };
            }

            var isValid = await _userManager.VerifyTwoFactorTokenAsync(
                user,
                _userManager.Options.Tokens.AuthenticatorTokenProvider,
                code);

            if (isValid)
            {
                await _userManager.ResetAccessFailedCountAsync(user);

                return new TwoFactorVerificationResult
                {
                    Succeeded = true
                };
            }

            var accessFailedResult = await _userManager.AccessFailedAsync(user);

            if (!accessFailedResult.Succeeded)
            {
                return new TwoFactorVerificationResult
                {
                    Succeeded = false
                };
            }

            var isLockedOut = await _userManager.IsLockedOutAsync(user);

            var lockoutEnd = isLockedOut
                    ? await _userManager.GetLockoutEndDateAsync(user)
                    : null;

            var failedCount = await _userManager.GetAccessFailedCountAsync(user);

            var maxAttempts = _userManager.Options.Lockout.MaxFailedAccessAttempts;

            var remainingAttempts = Math.Max(0, maxAttempts - failedCount);


            return new TwoFactorVerificationResult
            {
                Succeeded = false,
                LockedUntil = lockoutEnd,
                RemainingAttempts = remainingAttempts
            };
        }


        public async Task<bool> DisableTwoFactorAuthenticationAsync(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);

            if (user is null)
            {
                return false;
            }

            var twoFactorEnabled = await _userManager.GetTwoFactorEnabledAsync(user);

            if (!twoFactorEnabled)
            {
                return false;
            }

            var result = await _userManager.SetTwoFactorEnabledAsync(user, false);

            return result.Succeeded;
        }


        public async Task<IReadOnlyCollection<string>?> GenerateTwoFactorRecoveryCodesAsync(string userId,
                                                                                            int numberOfCodes)
        {
            var user = await _userManager.FindByIdAsync(userId);

            if (user is null)
            {
                return null;
            }

            var recoveryCodes = await _userManager.GenerateNewTwoFactorRecoveryCodesAsync(
                user, numberOfCodes);

            return recoveryCodes?.ToArray();
        }


        public async Task<TwoFactorConfirmationResult> ConfirmTwoFactorAsync(string userId,
                                                                             string code,
                                                                             int numberOfRecoveryCodes)
        {
            var user = await _userManager.FindByIdAsync(userId);

            if (user is null)
            {
                return new TwoFactorConfirmationResult
                {
                    Succeeded = false
                };
            }

            // Verify Totp code for this user
            var verificationResult = await VerifyTwoFactorCodeAsync(user, code);

            if (!verificationResult.Succeeded)
            {
                return new TwoFactorConfirmationResult
                {
                    Succeeded = false,
                    LockedUntil = verificationResult.LockedUntil,
                    RemainingAttempts = verificationResult.RemainingAttempts
                };
            }

            
            // Activate Two Factor Authentication for this user
            var enableResult = await _userManager.SetTwoFactorEnabledAsync(user, true);

            if (!enableResult.Succeeded)
            {
                return new TwoFactorConfirmationResult
                {
                    Succeeded = false
                };
            }

            var recoveryCodes = await _userManager.GenerateNewTwoFactorRecoveryCodesAsync(
                user, numberOfRecoveryCodes);

            if (recoveryCodes is null)
            {
                // 2FA was enabled, but recovery codes could not be generated.
                // Compensate by disabling 2FA.
                await _userManager.SetTwoFactorEnabledAsync(user, false);

                return new TwoFactorConfirmationResult
                {
                    Succeeded = false
                };
            }

            return new TwoFactorConfirmationResult
            {
                Succeeded = true,
                RecoveryCodes = recoveryCodes.ToArray()
            };
        }


        public async Task<bool> RedeemTwoFactorRecoveryCodeAsync(string userId,
                                                                 string recoveryCode)
        {
            var user = await _userManager.FindByIdAsync(userId);

            if (user is null)
            {
                return false;
            }

            var result = await _userManager.RedeemTwoFactorRecoveryCodeAsync(
                user, recoveryCode);

            return result.Succeeded;
        }
    }
}
