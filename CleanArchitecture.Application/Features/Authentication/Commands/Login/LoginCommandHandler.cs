using CleanArchitecture.Application.Common.Localization;
using CleanArchitecture.Application.Common.Localization.Resources;
using CleanArchitecture.Application.Common.Responses;
using CleanArchitecture.Application.Common.Services.Authentication;
using CleanArchitecture.Application.Common.Services.Authentication.Enums;
using CleanArchitecture.Application.Common.Services.Identity;
using MediatR;
using Microsoft.Extensions.Localization;

namespace CleanArchitecture.Application.Features.Authentication.Commands.Login
{
    public sealed class LoginCommandHandler : IRequestHandler<LoginCommand, Response<LoginResponse>>
    {
        private readonly IIdentityService _identityService;
        private readonly IJwtTokenService _jwtTokenService;
        private readonly IAuthenticationCompletionService _authenticationCompletionService;
        private readonly IStringLocalizer<SharedResources> _localizer;


        public LoginCommandHandler(IIdentityService identityService,
                                   IJwtTokenService jwtTokenService,
                                   IAuthenticationCompletionService authenticationCompletionService,
                                   IStringLocalizer<SharedResources> localizer)
        {
            this._identityService = identityService;
            this._jwtTokenService = jwtTokenService;
            this._authenticationCompletionService = authenticationCompletionService;
            this._localizer = localizer;
        }


        public async Task<Response<LoginResponse>> Handle(LoginCommand request, CancellationToken cancellationToken)
        {
            // Authenticate the user using the provided email and password
            var authenticationResult = await _identityService.AuthenticateAsync(
                request.Email, 
                request.Password, 
                cancellationToken);


            if (!authenticationResult.Succeeded)
            {
                return HandleAuthenticationFailure(authenticationResult.FailureReason, authenticationResult.RemainingAttempts, authenticationResult.LockedUntil);
            }

            var authenticatedUser = authenticationResult.User!;


            if (authenticationResult.RequiresTwoFactor)
            {
                // Generate purpose-specific 2FA JWT token
                var preAuthenticationToken = _jwtTokenService.GeneratePurposeToken(
                    authenticatedUser.Id.ToString(),
                    "2fa",
                    TimeSpan.FromMinutes(3));


                var twoFactorResponse = new TwoFactorChallengeResponse
                {
                    PreAuthenticationToken = preAuthenticationToken
                };


                var loginResponse = new LoginResponse
                {
                    RequiresTwoFactor = true,

                    TwoFactor = twoFactorResponse
                };


                return ResponseHandler.Success(
                    loginResponse,
                    _localizer[Messages.TwoFactorAuthenticationRequired]);
            }


            // Complete authentication process and generate tokens
            var tokenResponse = await _authenticationCompletionService.CompleteAuthenticationAsync(
                authenticatedUser, cancellationToken);
            

            var successfulLoginResponse = new LoginResponse
            {
                RequiresTwoFactor = false,

                Tokens = tokenResponse
            };


            return ResponseHandler.Success(
                successfulLoginResponse,
                _localizer[Messages.LoginSuccessfully]);
        }


        private Response<LoginResponse> HandleAuthenticationFailure(AuthenticationFailureReason failureReason, 
                                                                    int? remainingAttempts, 
                                                                    DateTimeOffset? lockedUntil)
        {
            return failureReason switch
            {
                AuthenticationFailureReason.InvalidCredentials =>
                    ResponseHandler.Unauthorized<LoginResponse>(
                        _localizer[Errors.InvalidCredentials],
                        errorCode: ErrorCodes.Authentication.InvalidCredentials,
                        meta: new
                        {
                            LockedUntil = lockedUntil,
                            RemainingAttempts = remainingAttempts
                        }),


                AuthenticationFailureReason.EmailNotConfirmed =>
                    ResponseHandler.Unauthorized<LoginResponse>(
                        _localizer[Errors.EmailNotConfirmed],
                        errorCode: ErrorCodes.Authentication.EmailNotConfirmed),


                AuthenticationFailureReason.AccountLocked =>
                    ResponseHandler.Locked<LoginResponse>(
                        _localizer[Errors.AccountLocked],
                        errorCode: ErrorCodes.Authentication.AccountLocked,
                        meta: new
                        {
                            LockedUntil = lockedUntil,
                            RemainingAttempts = 0
                        }),


                _ =>
                    ResponseHandler.Unauthorized<LoginResponse>(
                        _localizer[Errors.InvalidCredentials],
                        errorCode: ErrorCodes.Authentication.InvalidCredentials,
                        meta: new
                        {
                            LockedUntil = lockedUntil,
                            RemainingAttempts = remainingAttempts
                        })
            };
        }
    }
}
