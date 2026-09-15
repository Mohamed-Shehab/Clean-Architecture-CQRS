using CleanArchitecture.Application.Common.Responses;
using CleanArchitecture.Application.Features.Authentication.Models;
using MediatR;

namespace CleanArchitecture.Application.Features.Authentication.Commands.ValidateTwoFactor
{
    public sealed record ValidateTwoFactorCommand(string Code)
        : IRequest<Response<TokenResponse>>;
}
