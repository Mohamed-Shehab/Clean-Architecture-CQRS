using CleanArchitecture.Application.Common.Responses;
using MediatR;

namespace CleanArchitecture.Application.Features.Authentication.Commands.LogoutSession
{
    public sealed record LogoutSessionCommand(Guid UserSessionId) : IRequest<Response<object>>;
}
