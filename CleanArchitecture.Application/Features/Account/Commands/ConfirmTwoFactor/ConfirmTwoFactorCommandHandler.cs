using CleanArchitecture.Application.Common.Localization;
using CleanArchitecture.Application.Common.Localization.Resources;
using CleanArchitecture.Application.Common.Responses;
using CleanArchitecture.Application.Common.Services.CurrentUser;
using CleanArchitecture.Application.Common.Services.Identity;
using MediatR;
using Microsoft.Extensions.Localization;

namespace CleanArchitecture.Application.Features.Account.Commands.ConfirmTwoFactor
{
    public sealed class ConfirmTwoFactorCommandHandler : IRequestHandler<ConfirmTwoFactorCommand, Response<ConfirmTwoFactorResponse>>
    {
        private readonly ICurrentUserService _currentUserService;
        private readonly IIdentityService _identityService;
        private readonly IStringLocalizer<SharedResources> _localizer;


        public ConfirmTwoFactorCommandHandler(ICurrentUserService currentUserService,
                                              IIdentityService identityService,
                                              IStringLocalizer<SharedResources> localizer)
        {
            this._currentUserService = currentUserService;
            this._identityService = identityService;
            this._localizer = localizer;
        }


        public async Task<Response<ConfirmTwoFactorResponse>> Handle(ConfirmTwoFactorCommand request, CancellationToken cancellationToken)
        {
            var userId = _currentUserService.UserId;

            var result = await _identityService.ConfirmTwoFactorAsync(
                userId.ToString(), request.Code, 8);


            if (!result.Succeeded)
            {
                if (result.LockedUntil is not null)
                {
                    return ResponseHandler.Locked<ConfirmTwoFactorResponse>(
                        message: _localizer[Errors.AccountLocked],
                        errorCode: ErrorCodes.Authentication.AccountLocked,
                        meta: new
                        {
                            LockedUntil = result.LockedUntil,
                            RemainingAttempts = 0
                        });
                }


                return ResponseHandler.Unauthorized<ConfirmTwoFactorResponse>(
                    message: _localizer[Errors.InvalidTwoFactorAuthenticationCode],
                    errorCode: ErrorCodes.Authentication.InvalidTwoFactorCode,
                    meta: new
                    {
                        RemainingAttempts = result.RemainingAttempts
                    });
            }


            var response = new ConfirmTwoFactorResponse
            {
                RecoveryCodes = result.RecoveryCodes!
            };

            return ResponseHandler.Success(
                response,
                _localizer[Messages.TwoFactorAuthenticationEnabled]);
        }
    }
}
