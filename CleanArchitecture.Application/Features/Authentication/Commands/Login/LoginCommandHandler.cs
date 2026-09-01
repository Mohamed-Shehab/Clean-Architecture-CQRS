using CleanArchitecture.Application.Common.Interfaces;
using CleanArchitecture.Application.Common.Interfaces.Repositories;
using CleanArchitecture.Application.Common.Localization;
using CleanArchitecture.Application.Common.Localization.Resources;
using CleanArchitecture.Application.Common.Responses;
using CleanArchitecture.Application.Common.Services.Authentication;
using CleanArchitecture.Application.Common.Services.Authentication.Enums;
using CleanArchitecture.Application.Common.Services.ClientInfo;
using CleanArchitecture.Application.Common.Services.GeoLocation;
using CleanArchitecture.Application.Common.Services.Identity;
using CleanArchitecture.Application.Features.Authentication.Models;
using CleanArchitecture.Domain.Entities;
using MediatR;
using Microsoft.Extensions.Localization;

namespace CleanArchitecture.Application.Features.Authentication.Commands.Login
{
    public sealed class LoginCommandHandler : IRequestHandler<LoginCommand, Response<AuthenticationResponse>>
    {
        private readonly IIdentityService _identityService;
        private readonly IJwtTokenService _jwtTokenService;
        private readonly IRefreshTokenService _refreshTokenService;
        private readonly IClientInfoProvider _clientInfoProvider;
        private readonly IUserAgentParser _userAgentParser;
        private readonly IGeoLocationProvider _geoLocationProvider;
        private readonly IUserSessionRepository _userSessionRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IStringLocalizer<SharedResources> _localizer;


        public LoginCommandHandler(IIdentityService identityService,
                                   IJwtTokenService jwtTokenService,
                                   IRefreshTokenService refreshTokenService,
                                   IClientInfoProvider clientInfoProvider,
                                   IUserAgentParser userAgentParser,
                                   IGeoLocationProvider geoLocationProvider,
                                   IUserSessionRepository userSessionRepository,
                                   IUnitOfWork unitOfWork,
                                   IStringLocalizer<SharedResources> localizer)
        {
            this._identityService = identityService;
            this._jwtTokenService = jwtTokenService;
            this._refreshTokenService = refreshTokenService;
            this._clientInfoProvider = clientInfoProvider;
            this._userAgentParser = userAgentParser;
            this._geoLocationProvider = geoLocationProvider;
            this._userSessionRepository = userSessionRepository;
            this._unitOfWork = unitOfWork;
            this._localizer = localizer;
        }


        public async Task<Response<AuthenticationResponse>> Handle(LoginCommand request, CancellationToken cancellationToken)
        {
            // Authenticate the user using the provided email and password
            var authenticationResult = await _identityService.AuthenticateAsync(request.Email, 
                                                                                                 request.Password, 
                                                                                                 cancellationToken);

            if (!authenticationResult.Succeeded)
            {
                return HandleAuthenticationFailure(authenticationResult.FailureReason);
            }


            var authenticatedUser = authenticationResult.User!;


            // Generate access token
            var accessTokenResult = _jwtTokenService.GenerateAccessToken(authenticatedUser);


            // Generate refresh token
            var refreshTokenResult = _refreshTokenService.GenerateRefreshToken();


            #region Create a new user session for the authenticated device

            // Parse client device information
            var userAgent = _clientInfoProvider.UserAgent;

            var clientDeviceInfo = _userAgentParser.Parse(userAgent);


            // Resolve client location
            var ipAddress = _clientInfoProvider.IpAddress;

            var clientLocationInfo = _geoLocationProvider.GetLocation(ipAddress);


            var userSession = new UserSession
            {
                UserSessionId = Guid.CreateVersion7(),
                UserId = authenticatedUser.Id,

                RefreshTokenHash = _refreshTokenService.HashToken(refreshTokenResult.RefreshToken),
                RefreshTokenExpiresAt = refreshTokenResult.ExpiresAt,
                CreatedAt = DateTimeOffset.UtcNow,
                LastUsedAt = DateTimeOffset.UtcNow,

                // Client device information
                UserAgent = userAgent,
                DeviceType = clientDeviceInfo.DeviceType,
                OperatingSystem = clientDeviceInfo.OperatingSystem,
                Browser = clientDeviceInfo.Browser,

                // Client location information
                IpAddress = ipAddress,
                Country = clientLocationInfo.Country,
                Region = clientLocationInfo.Region,
                City = clientLocationInfo.City
            };

            await _userSessionRepository.AddAsync(userSession, cancellationToken);

            #endregion

            await _unitOfWork.SaveChangesAsync(cancellationToken);


            var authenticationResponse = new AuthenticationResponse
            {
                AccessToken = accessTokenResult.AccessToken,
                TokenType = "Bearer",
                AccessTokenExpiresAt = accessTokenResult.ExpiresAt,
                RefreshToken = refreshTokenResult.RefreshToken,
                RefreshTokenExpiresAt = refreshTokenResult.ExpiresAt
            };


            return ResponseHandler.Success(
                authenticationResponse,
                _localizer[Messages.LoginSuccessfully]);
        }


        private Response<AuthenticationResponse> HandleAuthenticationFailure(AuthenticationFailureReason failureReason)
        {
            return failureReason switch
            {
                AuthenticationFailureReason.InvalidCredentials =>
                    ResponseHandler.Unauthorized<AuthenticationResponse>(
                        _localizer[Errors.InvalidCredentials],
                        errorCode: ErrorCodes.Authentication.InvalidCredentials),


                AuthenticationFailureReason.EmailNotConfirmed =>
                    ResponseHandler.Unauthorized<AuthenticationResponse>(
                        _localizer[Errors.EmailNotConfirmed],
                        errorCode: ErrorCodes.Authentication.EmailNotConfirmed),


                AuthenticationFailureReason.AccountLocked =>
                    ResponseHandler.Unauthorized<AuthenticationResponse>(
                        _localizer[Errors.AccountLocked],
                        errorCode: ErrorCodes.Authentication.AccountLocked),


                _ =>
                    ResponseHandler.Unauthorized<AuthenticationResponse>(
                        _localizer[Errors.InvalidCredentials],
                        errorCode: ErrorCodes.Authentication.InvalidCredentials)
            };
        }
    }
}
