using CleanArchitecture.Application.Common.Services.Identity;
using CleanArchitecture.Application.Common.Services.Identity.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace CleanArchitecture.Infrastructure.Identity.Services
{
    public class IdentityService : IIdentityService
    {
        private readonly UserManager<ApplicationUser> _userManager;

        public IdentityService(UserManager<ApplicationUser> userManager)
        {
            this._userManager = userManager;
        }


        public Task<bool> IsEmailUsedAsync(string email, CancellationToken cancellationToken)
        {
            return _userManager.Users.AnyAsync(u => u.Email == email, cancellationToken);
        }

        public Task<bool> IsPhoneNumberUsedAsync(string phoneNumber, CancellationToken cancellationToken)
        {
            return _userManager.Users.AnyAsync(u => u.PhoneNumber == phoneNumber, cancellationToken);
        }
        public async Task<CreateUserResult> CreateUserAsync(string firstName, string lastName, string email, string phoneNumber, string password)
        {
            var user = new ApplicationUser()
            {
                FirstName = firstName,
                LastName = lastName,
                Email = email,
                PhoneNumber = phoneNumber,
                UserName = email
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
                Errors = new List<string>()
            };
        }

        public async Task<IdentityOperationResult> UpdateUserAsync(int userId,
                                                            string firstName,
                                                            string lastName,
                                                            string phoneNumber,
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

            user.FirstName = firstName;
            user.LastName = lastName;
            user.PhoneNumber = phoneNumber;

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

        public async Task<IdentityOperationResult> DeleteUserAsync(int userId, CancellationToken cancellationToken)
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

            var result = await _userManager.DeleteAsync(user);

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

    }
}
