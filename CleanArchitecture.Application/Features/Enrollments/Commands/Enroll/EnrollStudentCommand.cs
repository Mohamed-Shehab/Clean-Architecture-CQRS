using CleanArchitecture.Application.Common.Responses;
using MediatR;

namespace CleanArchitecture.Application.Features.Enrollments.Commands.Enroll
{
    public sealed record EnrollStudentCommand(int StudentId,
                                              int CourseId) : IRequest<Response<object>>;
}
