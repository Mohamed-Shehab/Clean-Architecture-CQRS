using CleanArchitecture.Application.Common.Responses;
using MediatR;

namespace CleanArchitecture.Application.Features.Authentication.Commands.LogoutAllSessions
{
    public sealed record LogoutAllSessionsCommand : IRequest<Response<object>>;
}
