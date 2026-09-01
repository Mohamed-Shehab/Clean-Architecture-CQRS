using CleanArchitecture.Application.Common.Interfaces;
using CleanArchitecture.Application.Common.Interfaces.Repositories;
using CleanArchitecture.Application.Common.Localization;
using CleanArchitecture.Application.Common.Localization.Resources;
using CleanArchitecture.Application.Common.Responses;
using CleanArchitecture.Application.Common.Services.Authentication;
using CleanArchitecture.Application.Common.Services.CurrentUser;
using MediatR;
using Microsoft.Extensions.Localization;

namespace CleanArchitecture.Application.Features.Authentication.Commands.Logout
{
    public sealed class LogoutCommandHandler : IRequestHandler<LogoutCommand, Response<object>>
    {
        private readonly IRefreshTokenService _refreshTokenService;
        private readonly IUserSessionRepository _userSessionRepository;
        private readonly ICurrentUserService _currentUserService;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IStringLocalizer<SharedResources> _localizer;


        public LogoutCommandHandler(IRefreshTokenService refreshTokenService,
                                    IUserSessionRepository userSessionRepository,
                                    ICurrentUserService currentUserService,
                                    IUnitOfWork unitOfWork,
                                    IStringLocalizer<SharedResources> localizer)
        {
            this._refreshTokenService = refreshTokenService;
            this._userSessionRepository = userSessionRepository;
            this._currentUserService = currentUserService;
            this._unitOfWork = unitOfWork;
            this._localizer = localizer;
        }


        public async Task<Response<object>> Handle(LogoutCommand request, CancellationToken cancellationToken)
        {
            // Hash the raw refresh token received from the client
            var refreshTokenHash = _refreshTokenService.HashToken(request.RefreshToken);


            // Find the session associated with this refresh token
            var userSession = await _userSessionRepository.GetByRefreshTokenHashAsync(refreshTokenHash, cancellationToken);


            // The session is already logged out or does not belong to the current user
            if (userSession is null || 
                userSession.UserId != _currentUserService.UserId || 
                userSession.IsRevoked)
            {
                return ResponseHandler.Success<object>(
                    message: _localizer[Messages.LogoutSuccessfully]);
            }


            // Revoke the current session
            userSession.RevokedAt = DateTimeOffset.UtcNow;


            await _unitOfWork.SaveChangesAsync(cancellationToken);


            return ResponseHandler.Success<object>(
                message: _localizer[Messages.LogoutSuccessfully]);
        }
    }
}
