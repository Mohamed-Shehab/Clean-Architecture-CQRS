using CleanArchitecture.Application.Common.Responses;
using MediatR;

namespace CleanArchitecture.Application.Features.Enrollments.Commands.Unenroll
{
    public sealed record UnenrollStudentCommand(int StudentId,
                                                int CourseId) : IRequest<Response<object>>;
}
