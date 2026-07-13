using CleanArchitecture.Application.Common.Responses;
using MediatR;

namespace CleanArchitecture.Application.Features.Courses.Queries.GetById.Public
{
    public sealed record GetCourseByIdQuery(int Id) : IRequest<Response<CourseDetailsDto>>;
}
