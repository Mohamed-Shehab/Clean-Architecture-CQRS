using CleanArchitecture.Application.Common.Models.Querying;
using CleanArchitecture.Application.Common.Responses;
using CleanArchitecture.Application.Features.Students.Queries.Get.Models;
using MediatR;

namespace CleanArchitecture.Application.Features.Students.Queries.Get
{
    public sealed record GetStudentsQuery(PaginationModel Pagination,
                                          StudentFilterModel? Filter = null,
                                          StudentSortingModel? Sorting = null) : IRequest<Response<List<StudentDto>>>;
}
