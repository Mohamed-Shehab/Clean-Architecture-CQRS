using CleanArchitecture.Application.Common.Localization;
using CleanArchitecture.Application.Common.Localization.Resources;
using CleanArchitecture.Application.Common.Responses;
using CleanArchitecture.Application.Common.Services.CurrentUser;
using CleanArchitecture.Application.Common.Services.Identity;
using MediatR;
using Microsoft.Extensions.Localization;

namespace CleanArchitecture.Application.Features.Account.Commands.DisableTwoFactor
{
    public sealed class DisableTwoFactorCommandHandler : IRequestHandler<DisableTwoFactorCommand, Response<object>>
    {
        private readonly ICurrentUserService _currentUserService;
        private readonly IIdentityService _identityService;
        private readonly IStringLocalizer<SharedResources> _localizer;


        public DisableTwoFactorCommandHandler(ICurrentUserService currentUserService,
                                              IIdentityService identityService,
                                              IStringLocalizer<SharedResources> localizer)
        {
            this._currentUserService = currentUserService;
            this._identityService = identityService;
            this._localizer = localizer;
        }


        public async Task<Response<object>> Handle(DisableTwoFactorCommand request, CancellationToken cancellationToken)
        {
            var userId = _currentUserService.UserId;

            // Verify Totp for this user
            var verificationResult = await _identityService.VerifyTwoFactorCodeAsync(
                userId.ToString(), request.Code);

            if (!verificationResult.Succeeded)
            {
                if (verificationResult.LockedUntil is not null)
                {
                    return ResponseHandler.Locked<object>(
                        message: _localizer[Errors.AccountLocked],
                        errorCode:
                            ErrorCodes.Authentication.AccountLocked,
                        meta: new
                        {
                            LockedUntil = verificationResult.LockedUntil,
                            RemainingAttempts = 0
                        });
                }


                return ResponseHandler.Unauthorized<object>(
                    message: _localizer[Errors.InvalidTwoFactorAuthenticationCode],
                    errorCode: ErrorCodes.Authentication.InvalidTwoFactorCode,
                    meta: new
                    {
                        RemainingAttempts = verificationResult.RemainingAttempts
                    });
            }


            var disabled = await _identityService.DisableTwoFactorAuthenticationAsync(userId.ToString());

            if (!disabled)
            {
                return ResponseHandler.InternalServerError<object>(
                    _localizer["SomethingWentWrong"]);
            }


            return ResponseHandler.Success<object>(
                message: _localizer[Messages.TwoFactorAuthenticationDisabledSuccessfully]);
        }
    }
}
