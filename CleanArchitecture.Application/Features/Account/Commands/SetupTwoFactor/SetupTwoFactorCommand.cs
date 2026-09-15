using CleanArchitecture.Application.Common.Responses;
using MediatR;

namespace CleanArchitecture.Application.Features.Account.Commands.SetupTwoFactor
{
    public sealed record SetupTwoFactorCommand : IRequest<Response<SetupTwoFactorResponse>>;
}
