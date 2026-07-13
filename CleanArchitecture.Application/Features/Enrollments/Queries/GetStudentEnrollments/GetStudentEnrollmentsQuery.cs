using CleanArchitecture.Application.Common.Models.Querying;
using CleanArchitecture.Application.Common.Responses;
using CleanArchitecture.Application.Features.Enrollments.Queries.GetStudentEnrollments.Models;
using MediatR;

namespace CleanArchitecture.Application.Features.Enrollments.Queries.GetStudentEnrollments
{
    public sealed record GetStudentEnrollmentsQuery(int StudentId,
                                                   PaginationModel Pagination,
                                                   StudentEnrollmentFilterModel? Filter,
                                                   StudentEnrollmentSortingModel? Sorting) : IRequest<Response<List<StudentEnrollmentDto>>>;
}
