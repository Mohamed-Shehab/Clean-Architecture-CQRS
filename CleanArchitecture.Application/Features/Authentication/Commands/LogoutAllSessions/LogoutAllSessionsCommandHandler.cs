using CleanArchitecture.Application.Common.Interfaces;
using CleanArchitecture.Application.Common.Interfaces.Repositories;
using CleanArchitecture.Application.Common.Localization;
using CleanArchitecture.Application.Common.Localization.Resources;
using CleanArchitecture.Application.Common.Responses;
using CleanArchitecture.Application.Common.Services.CurrentUser;
using MediatR;
using Microsoft.Extensions.Localization;

namespace CleanArchitecture.Application.Features.Authentication.Commands.LogoutAllSessions
{
    public sealed class LogoutAllSessionsCommandHandler : IRequestHandler<LogoutAllSessionsCommand, Response<object>>
    {
        private readonly ICurrentUserService _currentUserService;
        private readonly IUserSessionRepository _userSessionRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IStringLocalizer<SharedResources> _localizer;


        public LogoutAllSessionsCommandHandler(ICurrentUserService currentUserService,
                                               IUserSessionRepository userSessionRepository,
                                               IUnitOfWork unitOfWork,
                                               IStringLocalizer<SharedResources> localizer)
        {
            this._currentUserService = currentUserService;
            this._userSessionRepository = userSessionRepository;
            this._unitOfWork = unitOfWork;
            this._localizer = localizer;
        }


        public async Task<Response<object>> Handle(LogoutAllSessionsCommand request, CancellationToken cancellationToken)
        {
            var userId = _currentUserService.UserId;

            var userSessions = await _userSessionRepository.GetActiveSessionsEntitiesByUserIdAsync(
                userId, cancellationToken);


            // Revoke all active sessions for the user
            var revokedAt = DateTimeOffset.UtcNow;

            foreach (var session in userSessions)
            {
                session.RevokedAt = revokedAt;
            }


            await _unitOfWork.SaveChangesAsync(cancellationToken);


            return ResponseHandler.Success<object>(
                message: _localizer[Messages.LogoutSuccessfully]);
        }
    }
}
