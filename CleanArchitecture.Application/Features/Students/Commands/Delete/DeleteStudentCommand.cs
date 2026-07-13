using CleanArchitecture.Application.Common.Responses;
using MediatR;

namespace CleanArchitecture.Application.Features.Students.Commands.Delete
{
    public sealed record DeleteStudentCommand(int Id) : IRequest<Response<object>>;
}
