using CleanArchitecture.Application.Common.Responses;
using MediatR;

namespace CleanArchitecture.Application.Features.Account.Commands.DisableTwoFactor
{
    public sealed record DisableTwoFactorCommand(string Code)
        : IRequest<Response<object>>;
}
