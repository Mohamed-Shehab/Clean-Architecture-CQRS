using CleanArchitecture.Application.Common.Interfaces;
using CleanArchitecture.Application.Common.Interfaces.Repositories;
using CleanArchitecture.Application.Common.Localization;
using CleanArchitecture.Application.Common.Localization.Resources;
using CleanArchitecture.Application.Common.Responses;
using CleanArchitecture.Application.Common.Services.Authentication;
using CleanArchitecture.Application.Common.Services.Identity;
using CleanArchitecture.Application.Features.Authentication.Models;
using MediatR;
using Microsoft.Extensions.Localization;

namespace CleanArchitecture.Application.Features.Authentication.Commands.RefreshToken
{
    public sealed class RefreshTokenCommandHandler : IRequestHandler<RefreshTokenCommand, Response<AuthenticationResponse>>
    {
        private readonly IRefreshTokenService _refreshTokenService;
        private readonly IUserSessionRepository _userSessionRepository;
        private readonly IIdentityService _identityService;
        private readonly IJwtTokenService _jwtTokenService;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IStringLocalizer<SharedResources> _localizer;

        public RefreshTokenCommandHandler(IRefreshTokenService refreshTokenService,
                                          IUserSessionRepository userSessionRepository,
                                          IIdentityService identityService,
                                          IJwtTokenService jwtTokenService,
                                          IUnitOfWork unitOfWork,
                                          IStringLocalizer<SharedResources> localizer)
        {
            this._refreshTokenService = refreshTokenService;
            this._userSessionRepository = userSessionRepository;
            this._identityService = identityService;
            this._jwtTokenService = jwtTokenService;
            this._unitOfWork = unitOfWork;
            this._localizer = localizer;
        }


        public async Task<Response<AuthenticationResponse>> Handle(RefreshTokenCommand request, CancellationToken cancellationToken)
        {
            // Hash the raw refresh token received from the client
            var refreshTokenHash = _refreshTokenService.HashToken(request.RefreshToken);


            // Find the session associated with this refresh token
            var userSession = await _userSessionRepository.GetByRefreshTokenHashAsync(refreshTokenHash, cancellationToken);

            if (userSession is null)
            {
                return ResponseHandler.Unauthorized<AuthenticationResponse>(
                    _localizer[Errors.InvalidRefreshToken],
                    errorCode: ErrorCodes.Authentication.InvalidRefreshToken);
            }


            // Make sure the session has not been revoked
            if (userSession.IsRevoked)
            {
                return ResponseHandler.Unauthorized<AuthenticationResponse>(
                    _localizer[Errors.RefreshTokenRevoked],
                    errorCode: ErrorCodes.Authentication.RefreshTokenRevoked);
            }


            // Make sure the refresh token has not expired
            if (userSession.IsExpired)
            {
                return ResponseHandler.Unauthorized<AuthenticationResponse>(
                    _localizer[Errors.RefreshTokenExpired],
                    errorCode: ErrorCodes.Authentication.RefreshTokenExpired);
            }


            // Retrieve the user associated with this session
            var user = await _identityService.GetUserByIdAsync(userSession.UserId, cancellationToken);

            if (user is null)
            {
                return ResponseHandler.Unauthorized<AuthenticationResponse>(
                    _localizer[Errors.InvalidRefreshToken],
                    errorCode: ErrorCodes.Authentication.InvalidRefreshToken);
            }


            // Generate a new access token
            var accessTokenResult = _jwtTokenService.GenerateAccessToken(user);


            // Rotate the refresh token
            var newRefreshTokenResult = _refreshTokenService.GenerateRefreshToken();


            userSession.RefreshTokenHash = _refreshTokenService.HashToken(newRefreshTokenResult.RefreshToken);

            userSession.RefreshTokenExpiresAt = newRefreshTokenResult.ExpiresAt;

            userSession.LastUsedAt = DateTime.UtcNow;


            await _unitOfWork.SaveChangesAsync(cancellationToken);


            var authenticationResponse = new AuthenticationResponse
            {
                AccessToken = accessTokenResult.AccessToken,
                TokenType = "Bearer",
                AccessTokenExpiresAt = accessTokenResult.ExpiresAt,
                RefreshToken = newRefreshTokenResult.RefreshToken,
                RefreshTokenExpiresAt = newRefreshTokenResult.ExpiresAt
            };



            return ResponseHandler.Success(
                authenticationResponse,
                _localizer[Messages.TokenRefreshedSuccessfully]);
        }
    }
}
