using CleanArchitecture.Application.Common.Interfaces;
using CleanArchitecture.Application.Common.Interfaces.Repositories;
using CleanArchitecture.Application.Common.Services.Authentication.Models;
using CleanArchitecture.Application.Common.Services.ClientInfo;
using CleanArchitecture.Application.Common.Services.GeoLocation;
using CleanArchitecture.Application.Features.Authentication.Models;
using CleanArchitecture.Domain.Entities;

namespace CleanArchitecture.Application.Common.Services.Authentication
{
    public sealed class AuthenticationCompletionService : IAuthenticationCompletionService
    {
        private readonly IJwtTokenService _jwtTokenService;
        private readonly IRefreshTokenService _refreshTokenService;
        private readonly IClientInfoProvider _clientInfoProvider;
        private readonly IUserAgentParser _userAgentParser;
        private readonly IGeoLocationProvider _geoLocationProvider;
        private readonly IUserSessionRepository _userSessionRepository;
        private readonly IUnitOfWork _unitOfWork;


        public AuthenticationCompletionService(IJwtTokenService jwtTokenService,
                                               IRefreshTokenService refreshTokenService,
                                               IClientInfoProvider clientInfoProvider,
                                               IUserAgentParser userAgentParser,
                                               IGeoLocationProvider geoLocationProvider,
                                               IUserSessionRepository userSessionRepository,
                                               IUnitOfWork unitOfWork)
        {
            _jwtTokenService = jwtTokenService;
            _refreshTokenService = refreshTokenService;
            _clientInfoProvider = clientInfoProvider;
            _userAgentParser = userAgentParser;
            _geoLocationProvider = geoLocationProvider;
            _userSessionRepository = userSessionRepository;
            _unitOfWork = unitOfWork;
        }


        public async Task<TokenResponse> CompleteAuthenticationAsync(AuthenticatedUser user, CancellationToken cancellationToken)
        {
            // Generate access token
            var accessTokenResult = _jwtTokenService.GenerateAccessToken(user);


            // Generate refresh token
            var refreshTokenResult = _refreshTokenService.GenerateRefreshToken();


            #region Create a new user session for the authenticated device

            // Parse client device information
            var userAgent = _clientInfoProvider.UserAgent;

            var clientDeviceInfo = _userAgentParser.Parse(userAgent);


            // Resolve client location
            var ipAddress = _clientInfoProvider.IpAddress;

            var clientLocationInfo = _geoLocationProvider.GetLocation(ipAddress);


            var utcNow = DateTimeOffset.UtcNow;

            var userSession = new UserSession
            {
                UserSessionId = Guid.CreateVersion7(),
                UserId = user.Id,

                RefreshTokenHash = _refreshTokenService.HashToken(refreshTokenResult.RefreshToken),
                RefreshTokenExpiresAt = refreshTokenResult.ExpiresAt,
                CreatedAt = utcNow,
                LastUsedAt = utcNow,

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


            return new TokenResponse
            {
                AccessToken = accessTokenResult.AccessToken,
                TokenType = "Bearer",
                AccessTokenExpiresAt = accessTokenResult.ExpiresAt,
                RefreshToken = refreshTokenResult.RefreshToken,
                RefreshTokenExpiresAt = refreshTokenResult.ExpiresAt
            };
        }
    }
}
