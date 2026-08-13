using CleanArchitecture.Application.Common.Localization;
using CleanArchitecture.Application.Common.Localization.Resources;
using CleanArchitecture.Application.Common.Responses;
using CleanArchitecture.Application.Common.Services.CurrentUser;
using CleanArchitecture.Application.Common.Services.Identity;
using MediatR;
using Microsoft.Extensions.Localization;

namespace CleanArchitecture.Application.Features.Account.Commands.ChangePassword
{
    public sealed class ChangePasswordCommandHandler : IRequestHandler<ChangePasswordCommand, Response<object>>
    {
        private readonly IIdentityService _identityService;
        private readonly ICurrentUserService _currentUserService;
        private readonly IStringLocalizer<SharedResources> _localizer;

        public ChangePasswordCommandHandler(IIdentityService identityService,
                                            ICurrentUserService currentUserService,
                                            IStringLocalizer<SharedResources> localizer)
        {
            this._identityService = identityService;
            this._currentUserService = currentUserService;
            this._localizer = localizer;
        }


        public async Task<Response<object>> Handle(ChangePasswordCommand request, CancellationToken cancellationToken)
        {
            // Get the ID of the currently authenticated user
            int userId = _currentUserService.UserId;

            // Change the user's password
            var changePasswordResult = await _identityService.ChangePasswordAsync(
                userId, 
                request.CurrentPassword, 
                request.NewPassword,
                cancellationToken);


            if (!changePasswordResult.Succeeded)
            {
                return ResponseHandler.BadRequest<object>(
                    message: _localizer[Messages.PasswordChangeFailed],
                    errorCode: ErrorCodes.Account.PasswordChangeFailed,
                    errors: changePasswordResult.Errors);
            }



            return ResponseHandler.Success<object>(
                message: _localizer[Messages.PasswordChangedSuccessfully]);
        }
    }
}
