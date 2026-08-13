using CleanArchitecture.Application.Common.Responses;
using MediatR;

namespace CleanArchitecture.Application.Features.Account.Commands.ChangeEmail
{
    public sealed record ChangeEmailCommand(
        string CurrentPassword,
        string NewEmail) : IRequest<Response<object>>;
}
