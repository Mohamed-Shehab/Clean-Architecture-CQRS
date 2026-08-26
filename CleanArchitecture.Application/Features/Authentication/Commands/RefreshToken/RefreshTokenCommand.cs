using CleanArchitecture.Application.Common.Responses;
using CleanArchitecture.Application.Features.Authentication.Models;
using MediatR;

namespace CleanArchitecture.Application.Features.Authentication.Commands.RefreshToken
{
    public sealed record RefreshTokenCommand(string RefreshToken) : IRequest<Response<AuthenticationResponse>>;
}
