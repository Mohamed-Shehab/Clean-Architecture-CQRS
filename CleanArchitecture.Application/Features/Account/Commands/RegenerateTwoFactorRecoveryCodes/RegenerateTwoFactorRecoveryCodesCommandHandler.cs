using CleanArchitecture.Application.Common.Localization;
using CleanArchitecture.Application.Common.Localization.Resources;
using CleanArchitecture.Application.Common.Responses;
using CleanArchitecture.Application.Common.Services.CurrentUser;
using CleanArchitecture.Application.Common.Services.Identity;
using MediatR;
using Microsoft.Extensions.Localization;

namespace CleanArchitecture.Application.Features.Account.Commands.RegenerateTwoFactorRecoveryCodes
{
    public sealed class RegenerateTwoFactorRecoveryCodesCommandHandler
        : IRequestHandler<RegenerateTwoFactorRecoveryCodesCommand, Response<RegenerateTwoFactorRecoveryCodesResponse>>
    {
        private readonly ICurrentUserService _currentUserService;
        private readonly IIdentityService _identityService;
        private readonly IStringLocalizer<SharedResources> _localizer;


        public RegenerateTwoFactorRecoveryCodesCommandHandler(ICurrentUserService currentUserService,
                                                              IIdentityService identityService,
                                                              IStringLocalizer<SharedResources> localizer)
        {
            this._currentUserService = currentUserService;
            this._identityService = identityService;
            this._localizer = localizer;
        }


        public async Task<Response<RegenerateTwoFactorRecoveryCodesResponse>> Handle(RegenerateTwoFactorRecoveryCodesCommand request, CancellationToken cancellationToken)
        {
            var userId = _currentUserService.UserId;

            // Verify Totp for this user
            var verificationResult = await _identityService.VerifyTwoFactorCodeAsync(
                userId.ToString(), request.Code);

            if (!verificationResult.Succeeded)
            {
                if (verificationResult.LockedUntil is not null)
                {
                    return ResponseHandler.Locked<RegenerateTwoFactorRecoveryCodesResponse>(
                        message: _localizer[Errors.AccountLocked],
                        errorCode:
                            ErrorCodes.Authentication.AccountLocked,
                        meta: new
                        {
                            LockedUntil = verificationResult.LockedUntil,
                            RemainingAttempts = 0
                        });
                }


                return ResponseHandler.Unauthorized<RegenerateTwoFactorRecoveryCodesResponse>(
                    message: _localizer[Errors.InvalidTwoFactorAuthenticationCode],
                    errorCode: ErrorCodes.Authentication.InvalidTwoFactorCode,
                    meta: new
                    {
                        RemainingAttempts = verificationResult.RemainingAttempts
                    });
            }


            // Regenerate new list of recovery codes for this user
            var recoveryCodes = await _identityService.GenerateTwoFactorRecoveryCodesAsync(userId.ToString(), 8);

            if (recoveryCodes is null)
            {
                return ResponseHandler.InternalServerError<RegenerateTwoFactorRecoveryCodesResponse>(
                    _localizer["SomethingWentWrong"]);
            }

            var response = new RegenerateTwoFactorRecoveryCodesResponse
            {
                RecoveryCodes = recoveryCodes
            };


            return ResponseHandler.Success(
                response,
                _localizer[Messages.TwoFactorRecoveryCodesRegeneratedSuccessfully]);
        }
    }
}
