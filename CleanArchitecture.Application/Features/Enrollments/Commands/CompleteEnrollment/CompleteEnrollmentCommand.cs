using CleanArchitecture.Application.Common.Responses;
using MediatR;

namespace CleanArchitecture.Application.Features.Enrollments.Commands.CompleteEnrollment
{
    public sealed record CompleteEnrollmentCommand(int StudentId,
                                                   int CourseId) : IRequest<Response<object>>; 
}
