using CleanArchitecture.Application.Common.Responses;
using MediatR;

namespace CleanArchitecture.Application.Features.Account.Commands.ConfirmTwoFactor
{
    public sealed record ConfirmTwoFactorCommand(string Code) : IRequest<Response<ConfirmTwoFactorResponse>>;

}
