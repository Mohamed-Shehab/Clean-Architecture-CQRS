using CleanArchitecture.Application.Common.Models.Querying;
using CleanArchitecture.Application.Common.Responses;
using CleanArchitecture.Application.Features.Courses.Queries.Get.Management.Models;
using MediatR;

namespace CleanArchitecture.Application.Features.Courses.Queries.Get.Management
{
    public sealed record GetCoursesManagementQuery(PaginationModel Pagination,
                                         CourseManagementFilterModel? Filter,
                                         CourseManagementSortingModel? Sorting) : IRequest<Response<List<CourseManagementDto>>>;
}
