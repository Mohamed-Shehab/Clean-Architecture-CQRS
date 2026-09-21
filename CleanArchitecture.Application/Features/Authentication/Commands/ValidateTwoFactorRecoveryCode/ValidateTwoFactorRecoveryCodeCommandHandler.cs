using CleanArchitecture.Application.Common.Localization;
using CleanArchitecture.Application.Common.Localization.Resources;
using CleanArchitecture.Application.Common.Responses;
using CleanArchitecture.Application.Common.Services.Authentication;
using CleanArchitecture.Application.Common.Services.CurrentUser;
using CleanArchitecture.Application.Common.Services.Identity;
using CleanArchitecture.Application.Features.Authentication.Models;
using MediatR;
using Microsoft.Extensions.Localization;

namespace CleanArchitecture.Application.Features.Authentication.Commands.ValidateTwoFactorRecoveryCode
{
    public sealed class ValidateTwoFactorRecoveryCodeCommandHandler 
        : IRequestHandler<ValidateTwoFactorRecoveryCodeCommand, Response<TokenResponse>>
    {
        private readonly ICurrentUserService _currentUserService;
        private readonly IIdentityService _identityService;
        private readonly IAuthenticationCompletionService _authenticationCompletionService;
        private readonly IStringLocalizer<SharedResources> _localizer;


        public ValidateTwoFactorRecoveryCodeCommandHandler(ICurrentUserService currentUserService,
                                                           IIdentityService identityService,
                                                           IAuthenticationCompletionService authenticationCompletionService,
                                                           IStringLocalizer<SharedResources> localizer)
        {
            this._currentUserService = currentUserService;
            this._identityService = identityService;
            this._authenticationCompletionService = authenticationCompletionService;
            this._localizer = localizer;
        }

        public async Task<Response<TokenResponse>> Handle(ValidateTwoFactorRecoveryCodeCommand request, CancellationToken cancellationToken)
        {
            var userId = _currentUserService.UserId;

            var redeemed = await _identityService.RedeemTwoFactorRecoveryCodeAsync(userId.ToString(), request.RecoveryCode);

            if (!redeemed)
            {
                return ResponseHandler.Unauthorized<TokenResponse>(
                    message: _localizer[Errors.InvalidTwoFactorAuthenticationRecoveryCode],
                    errorCode: ErrorCodes.Authentication.InvalidTwoFactorRecoveryCode);
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
