using CleanArchitecture.Application.Common.DTOs;
using CleanArchitecture.Application.Common.Responses;
using CleanArchitecture.Application.Features.Students.DTOs;
using MediatR;

namespace CleanArchitecture.Application.Features.Students.Queries.Get
{
    public sealed record GetStudentsQuery(PaginationParams Pagination,
                                          FilterParams? Filter = null,
                                          SortingParams? Sorting = null) : IRequest<Response<List<StudentDto>>>
    {
    }
}
