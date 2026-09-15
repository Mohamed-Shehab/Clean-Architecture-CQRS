using CleanArchitecture.Application.Common.Localization;
using CleanArchitecture.Application.Common.Localization.Resources;
using CleanArchitecture.Application.Common.Responses;
using CleanArchitecture.Application.Common.Services.CurrentUser;
using CleanArchitecture.Application.Common.Services.Identity;
using CleanArchitecture.Application.Common.Services.QrCode;
using MediatR;
using Microsoft.Extensions.Localization;

namespace CleanArchitecture.Application.Features.Account.Commands.SetupTwoFactor
{
    public sealed class SetupTwoFactorCommandHandler : IRequestHandler<SetupTwoFactorCommand, Response<SetupTwoFactorResponse>>
    {
        private readonly ICurrentUserService _currentUserService;
        private readonly IIdentityService _identityService;
        private readonly IQrCodeService _qrCodeService;
        private readonly IStringLocalizer<SharedResources> _localizer;


        public SetupTwoFactorCommandHandler(ICurrentUserService currentUserService,
                                            IIdentityService identityService,
                                            IQrCodeService qrCodeService,
                                            IStringLocalizer<SharedResources> localizer)
        {
            this._currentUserService = currentUserService;
            this._identityService = identityService;
            this._qrCodeService = qrCodeService;
            this._localizer = localizer;
        }


        public async Task<Response<SetupTwoFactorResponse>> Handle(SetupTwoFactorCommand request, CancellationToken cancellationToken)
        {
            var userId = _currentUserService.UserId;

            // Generate a new authenticator key for the user
            var authenticatorKey = await _identityService.GenerateAuthenticatorKeyAsync(userId.ToString());

            if (authenticatorKey is null)
            {
                return ResponseHandler.NotFound<SetupTwoFactorResponse>(
                    _localizer[Messages.NotFound, _localizer[Entities.User]],
                    errorCode: ErrorCodes.Authentication.SessionNoLongerValid);
            }

            var issuer = "CleanArchitecture";
            var accountName = _currentUserService.Email;

            string otpUri =
                $"otpauth://totp/{Uri.EscapeDataString(issuer)}:{Uri.EscapeDataString(accountName)}" +
                $"?secret={authenticatorKey}" +
                $"&issuer={Uri.EscapeDataString(issuer)}" +
                $"&digits=6" +
                $"&period=30";

            var qrCodeImage = _qrCodeService.Generate(otpUri);

            var qrCodeImageBase64 = Convert.ToBase64String(qrCodeImage);


            var setupResponse = new SetupTwoFactorResponse
            {
                QrCodeImage = $"data:image/png;base64,{qrCodeImageBase64}"
            };


            return ResponseHandler.Success(
                setupResponse,
                _localizer[Messages.TwoFactorAuthenticationSetupInitiated]);
        }
    }
}
