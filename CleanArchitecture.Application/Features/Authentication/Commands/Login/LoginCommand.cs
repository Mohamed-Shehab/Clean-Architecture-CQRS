using CleanArchitecture.Application.Common.Responses;
using CleanArchitecture.Application.Features.Authentication.Models;
using MediatR;

namespace CleanArchitecture.Application.Features.Authentication.Commands.Login
{
    public sealed record LoginCommand(string Email, string Password) : IRequest<Response<AuthenticationResponse>>;
}
