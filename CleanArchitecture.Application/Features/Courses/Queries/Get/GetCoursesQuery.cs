using CleanArchitecture.Application.Common.DTOs;
using CleanArchitecture.Application.Common.Responses;
using CleanArchitecture.Application.Features.Courses.DTOs;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CleanArchitecture.Application.Features.Courses.Queries.Get
{
    public sealed record GetCoursesQuery(PaginationParams Pagination) : IRequest<Response<List<CourseDto>>>
    {
    }
}
