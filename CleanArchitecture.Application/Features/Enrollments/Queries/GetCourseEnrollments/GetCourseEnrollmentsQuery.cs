using CleanArchitecture.Application.Common.Models.Querying;
using CleanArchitecture.Application.Common.Responses;
using CleanArchitecture.Application.Features.Enrollments.Queries.GetCourseEnrollments.Models;
using MediatR;

namespace CleanArchitecture.Application.Features.Enrollments.Queries.GetCourseEnrollments
{
    public sealed record GetCourseEnrollmentsQuery(int CourseId,
                                                   PaginationModel Pagination,
                                                   CourseEnrollmentFilterModel? Filter,
                                                   CourseEnrollmentSortingModel? Sorting) : IRequest<Response<List<CourseEnrollmentDto>>>;
}
