using CleanArchitecture.Application.Common.Localization;
using CleanArchitecture.Application.Common.Localization.Resources;
using CleanArchitecture.Application.Common.Responses;
using CleanArchitecture.Application.Common.Services.Authentication;
using CleanArchitecture.Application.Common.Services.CurrentUser;
using CleanArchitecture.Application.Common.Services.Identity;
using CleanArchitecture.Application.Features.Authentication.Models;
using MediatR;
using Microsoft.Extensions.Localization;

namespace CleanArchitecture.Application.Features.Authentication.Commands.ValidateTwoFactor
{
    public sealed class ValidateTwoFactorCommandHandler : IRequestHandler<ValidateTwoFactorCommand, Response<TokenResponse>>
    {
        private readonly ICurrentUserService _currentUserService;
        private readonly IIdentityService _identityService;
        private readonly IAuthenticationCompletionService _authenticationCompletionService;
        private readonly IStringLocalizer<SharedResources> _localizer;


        public ValidateTwoFactorCommandHandler(ICurrentUserService currentUserService,
                                               IIdentityService identityService,
                                               IAuthenticationCompletionService authenticationCompletionService,
                                               IStringLocalizer<SharedResources> localizer)
        {
            this._currentUserService = currentUserService;
            this._identityService = identityService;
            this._authenticationCompletionService = authenticationCompletionService;
            this._localizer = localizer;
        }


        public async Task<Response<TokenResponse>> Handle(ValidateTwoFactorCommand request, CancellationToken cancellationToken)
        {
            var userId = _currentUserService.UserId;

            // Verify Totp for this user
            var verificationResult = await _identityService.VerifyTwoFactorCodeAsync(
                userId.ToString(), request.Code);


            if (!verificationResult.Succeeded)
            {
                if (verificationResult.LockedUntil is not null)
                {
                    return ResponseHandler.Locked<TokenResponse>(
                        message: _localizer[Errors.AccountLocked],
                        errorCode:
                            ErrorCodes.Authentication.AccountLocked,
                        meta: new
                        {
                            LockedUntil = verificationResult.LockedUntil,
                            RemainingAttempts = 0
                        });
                }


                return ResponseHandler.Unauthorized<TokenResponse>(
                    message: _localizer[Errors.InvalidTwoFactorAuthenticationCode],
                    errorCode: ErrorCodes.Authentication.InvalidTwoFactorCode,
                    meta: new
                    {
                        RemainingAttempts = verificationResult.RemainingAttempts
                    });
            }


            var authenticatedUser = await _identityService.CompleteLoginAsync(userId.ToString());


            if (authenticatedUser is null)
            {
                return ResponseHandler.NotFound<TokenResponse>(
                    _localizer[Messages.NotFound, _localizer[Entities.User]],
                    errorCode: ErrorCodes.Authentication.SessionNoLongerValid);
            }


            var tokenResponse = await _authenticationCompletionService.CompleteAuthenticationAsync(
                authenticatedUser, cancellationToken);


            return ResponseHandler.Success(
                tokenResponse,
                _localizer[Messages.LoginSuccessfully]);
        }
    }
}
