using CleanArchitecture.Application.Common.Responses;
using MediatR;

namespace CleanArchitecture.Application.Features.Courses.Commands.Delete
{
    public sealed record DeleteCourseCommand(int Id) : IRequest<Response<object>>;
}
