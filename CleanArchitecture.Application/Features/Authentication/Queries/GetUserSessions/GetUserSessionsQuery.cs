using CleanArchitecture.Application.Common.Responses;
using MediatR;

namespace CleanArchitecture.Application.Features.Authentication.Queries.GetUserSessions
{
    public sealed record GetUserSessionsQuery : IRequest<Response<List<UserSessionDto>>>;
}
