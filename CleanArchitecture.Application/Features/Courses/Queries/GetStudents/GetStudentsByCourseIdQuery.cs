using CleanArchitecture.Application.Common.DTOs;
using CleanArchitecture.Application.Common.Responses;
using CleanArchitecture.Application.Features.Courses.DTOs;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CleanArchitecture.Application.Features.Courses.Queries.GetStudents
{
    public sealed record GetStudentsByCourseIdQuery(int Id,
                                          PaginationParams Pagination,
                                          FilterParams? Filter = null,
                                          SortingParams? Sorting = null) : IRequest<Response<List<CourseStudentDto>>>
    {
    }
}
