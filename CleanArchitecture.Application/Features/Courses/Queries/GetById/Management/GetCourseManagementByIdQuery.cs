using CleanArchitecture.Application.Common.Responses;
using MediatR;

namespace CleanArchitecture.Application.Features.Courses.Queries.GetById.Management
{
    public sealed record GetCourseManagementByIdQuery(int Id) : IRequest<Response<CourseManagementDetailsDto>>;
}
