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
                    FailureReason = AuthenticationFailureReason.AccountLocked
                };
            }

            if (!passwordResult.Succeeded)
            {
                return new AuthenticationResult
                {
                    Succeeded = false,
                    FailureReason = AuthenticationFailureReason.InvalidCredentials
                };
            }


            var roles = await _userManager.GetRolesAsync(user);

            user.LastLoginAt = DateTime.UtcNow;
            await _userManager.UpdateAsync(user);

            return new AuthenticationResult
            {
                Succeeded = true,

                User = new AuthenticatedUser
                {
                    Id = user.Id,
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    Email = user.Email!,
                    Roles = roles.ToArray(),
                    Permissions = Array.Empty<string>() //todo: Permissions are not implemented yet
                },

                FailureReason = AuthenticationFailureReason.None
            };
        }


        public async Task<AuthenticatedUser?> GetUserByIdAsync(int userId, 
                                                               CancellationToken cancellationToken = default)
        {
            var user = await _userManager.Users
                .FirstOrDefaultAsync(u => u.Id == userId, cancellationToken);

            if (user is null)
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
    }
}
