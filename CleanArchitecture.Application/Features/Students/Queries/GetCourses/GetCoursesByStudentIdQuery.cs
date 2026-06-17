using CleanArchitecture.Application.Common.DTOs;
using CleanArchitecture.Application.Common.Responses;
using CleanArchitecture.Application.Features.Students.DTOs;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CleanArchitecture.Application.Features.Students.Queries.GetCourses
{
    public sealed record GetCoursesByStudentIdQuery(int StudentId,
                                                    PaginationParams Pagination,
                                                    FilterParams? Filter,
                                                    SortingParams? Sorting
    ) : IRequest<Response<List<StudentCourseDto>>>
    {
    }
}
