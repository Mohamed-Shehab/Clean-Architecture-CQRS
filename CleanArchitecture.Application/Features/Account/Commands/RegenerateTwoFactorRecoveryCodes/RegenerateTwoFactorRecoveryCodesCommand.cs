using CleanArchitecture.Application.Common.Responses;
using MediatR;

namespace CleanArchitecture.Application.Features.Account.Commands.RegenerateTwoFactorRecoveryCodes
{
    public sealed record RegenerateTwoFactorRecoveryCodesCommand(string Code) 
        : IRequest<Response<RegenerateTwoFactorRecoveryCodesResponse>>;
}
