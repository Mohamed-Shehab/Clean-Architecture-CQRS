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
        private readonly IStringLocalizer<SharedResources> _localizer;


        public LoginCommandHandler(IIdentityService identityService,
                                   IJwtTokenService jwtTokenService,
                                   IStringLocalizer<SharedResources> localizer)
        {
            this._identityService = identityService;
            this._jwtTokenService = jwtTokenService;
            this._localizer = localizer;
        }


        public async Task<Response<LoginResponse>> Handle(LoginCommand request, CancellationToken cancellationToken)
        {
            // Authenticate the user using the provided email and password
            var authenticationResult = await _identityService.AuthenticateAsync(request.Email, 
                                                                                                 request.Password, 
                                                                                                 cancellationToken);

            if (!authenticationResult.Succeeded)
            {
                return HandleAuthenticationFailure(authenticationResult.FailureReason);
            }


            // Generate access token and refresh token
            var accessTokenResult = _jwtTokenService.GenerateAccessToken(authenticationResult.User!);

            // Todo: Implement refresh token generation and storage
            //var refreshToken = _jwtTokenService.GenerateRefreshToken(authenticationResult.User!); 

            var loginResponse = new LoginResponse
            {
                AccessToken = accessTokenResult.AccessToken,
                TokenType = "Bearer",
                AccessTokenExpiresAt = accessTokenResult.ExpiresAt,
                RefreshToken = "" // Set the refresh token here when implemented
            };

            return ResponseHandler.Success(
                loginResponse,
                _localizer[Messages.LoginSuccessfully]);
        }


        private Response<LoginResponse> HandleAuthenticationFailure(AuthenticationFailureReason failureReason)
        {
            return failureReason switch
            {
                AuthenticationFailureReason.InvalidCredentials =>
                    ResponseHandler.Unauthorized<LoginResponse>(
                        _localizer[Errors.InvalidCredentials],
                        errorCode: ErrorCodes.Authentication.InvalidCredentials),


                AuthenticationFailureReason.EmailNotConfirmed =>
                    ResponseHandler.Unauthorized<LoginResponse>(
                        _localizer[Errors.EmailNotConfirmed],
                        errorCode: ErrorCodes.Authentication.EmailNotConfirmed),


                AuthenticationFailureReason.AccountLocked =>
                    ResponseHandler.Unauthorized<LoginResponse>(
                        _localizer[Errors.AccountLocked],
                        errorCode: ErrorCodes.Authentication.AccountLocked),


                _ =>
                    ResponseHandler.Unauthorized<LoginResponse>(
                        _localizer[Errors.InvalidCredentials],
                        errorCode: ErrorCodes.Authentication.InvalidCredentials)
            };
        }
    }
}
