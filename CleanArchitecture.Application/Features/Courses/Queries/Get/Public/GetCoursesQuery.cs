using CleanArchitecture.Application.Common.Models.Querying;
using CleanArchitecture.Application.Common.Responses;
using CleanArchitecture.Application.Features.Courses.Queries.Get.Public.Modles;
using MediatR;

namespace CleanArchitecture.Application.Features.Courses.Queries.Get.Public
{
    public sealed record GetCoursesQuery(PaginationModel Pagination,
                                         CourseFilterModel? Filter,
                                         CourseSortingModel? Sorting) : IRequest<Response<List<CourseDto>>>;
}
