using CleanArchitecture.Application.Common.Responses;
using CleanArchitecture.Application.Features.Courses.DTOs;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CleanArchitecture.Application.Features.Courses.Queries.GetById
{
    public sealed record GetCourseByIdQuery(int Id) : IRequest<Response<CourseDetailsDto>>
    {
    }
}
