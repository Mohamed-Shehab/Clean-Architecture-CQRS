using CleanArchitecture.Application.Common.Responses;
using MediatR;

namespace CleanArchitecture.Application.Features.Account.Commands.ChangePassword
{
    public sealed record ChangePasswordCommand(
        string CurrentPassword,
        string NewPassword,
        string ConfirmNewPassword) : IRequest<Response<object>>;
}
