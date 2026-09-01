using CleanArchitecture.Application.Common.Responses;
using MediatR;

namespace CleanArchitecture.Application.Features.Authentication.Commands.Logout
{
    public sealed record LogoutCommand(string RefreshToken) : IRequest<Response<object>>;
}
