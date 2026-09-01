using CleanArchitecture.Application.Common.Interfaces;
using CleanArchitecture.Application.Common.Interfaces.Repositories;
using CleanArchitecture.Application.Common.Localization;
using CleanArchitecture.Application.Common.Localization.Resources;
using CleanArchitecture.Application.Common.Responses;
using CleanArchitecture.Application.Common.Services.CurrentUser;
using MediatR;
using Microsoft.Extensions.Localization;

namespace CleanArchitecture.Application.Features.Authentication.Commands.LogoutSession
{
    public sealed class LogoutSessionCommandHandler : IRequestHandler<LogoutSessionCommand, Response<object>>
    {
        private readonly IUserSessionRepository _userSessionRepository;
        private readonly ICurrentUserService _currentUserService;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IStringLocalizer<SharedResources> _localizer;


        public LogoutSessionCommandHandler(IUserSessionRepository userSessionRepository,
                                           ICurrentUserService currentUserService,
                                           IUnitOfWork unitOfWork,
                                           IStringLocalizer<SharedResources> localizer)
        {
            this._userSessionRepository = userSessionRepository;
            this._currentUserService = currentUserService;
            this._unitOfWork = unitOfWork;
            this._localizer = localizer;
        }


        public async Task<Response<object>> Handle(LogoutSessionCommand request, CancellationToken cancellationToken)
        {
            var userSession = await _userSessionRepository.FirstOrDefaultAsync(
                us => us.UserSessionId == request.UserSessionId, cancellationToken);


            // The session does not exist or is already logged out.
            if (userSession is null || 
                userSession.UserId != _currentUserService.UserId ||
                userSession.IsRevoked)
            {
                return ResponseHandler.Success<object>(
                    message: _localizer[Messages.LogoutSuccessfully]);
            }


            userSession.RevokedAt = DateTimeOffset.UtcNow;


            await _unitOfWork.SaveChangesAsync(cancellationToken);


            return ResponseHandler.Success<object>(
                message: _localizer[Messages.LogoutSuccessfully]);
        }
    }
}
