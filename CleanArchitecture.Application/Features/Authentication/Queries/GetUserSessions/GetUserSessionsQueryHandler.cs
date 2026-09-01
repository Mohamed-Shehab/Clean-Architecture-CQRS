using CleanArchitecture.Application.Common.Interfaces.Repositories;
using CleanArchitecture.Application.Common.Localization;
using CleanArchitecture.Application.Common.Localization.Resources;
using CleanArchitecture.Application.Common.Responses;
using CleanArchitecture.Application.Common.Services.CurrentUser;
using MediatR;
using Microsoft.Extensions.Localization;

namespace CleanArchitecture.Application.Features.Authentication.Queries.GetUserSessions
{
    public sealed class GetUserSessionsQueryHandler : IRequestHandler<GetUserSessionsQuery, Response<List<UserSessionDto>>>
    {
        private readonly ICurrentUserService _currentUserService;
        private readonly IUserSessionRepository _userSessionRepository;
        private readonly IStringLocalizer<SharedResources> _localizer;
        

        public GetUserSessionsQueryHandler(ICurrentUserService currentUserService,
                                           IUserSessionRepository userSessionRepository,
                                           IStringLocalizer<SharedResources> localizer)
        {
            this._currentUserService = currentUserService;
            this._userSessionRepository = userSessionRepository;
            this._localizer = localizer;
        }


        public async Task<Response<List<UserSessionDto>>> Handle(GetUserSessionsQuery request, CancellationToken cancellationToken)
        {
            var userId = _currentUserService.UserId;

            var userSessions = await _userSessionRepository.GetActiveSessionsByUserIdAsync(userId, cancellationToken);


            return ResponseHandler.Success(
                userSessions,
                _localizer[Messages.UserSessionsRetrievedSuccessfully]);
        }
    }
}
