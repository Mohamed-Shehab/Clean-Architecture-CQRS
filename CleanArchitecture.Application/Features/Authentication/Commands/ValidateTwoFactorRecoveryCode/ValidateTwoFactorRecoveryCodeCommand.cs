using CleanArchitecture.Application.Common.Responses;
using CleanArchitecture.Application.Features.Authentication.Models;
using MediatR;

namespace CleanArchitecture.Application.Features.Authentication.Commands.ValidateTwoFactorRecoveryCode
{
    public sealed record ValidateTwoFactorRecoveryCodeCommand(string RecoveryCode)
        : IRequest<Response<TokenResponse>>;
}
