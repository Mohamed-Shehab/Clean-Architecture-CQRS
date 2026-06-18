using AutoMapper;
using CleanArchitecture.Application.Common.Helpers;
using CleanArchitecture.Application.Common.Interfaces.Repositories;
using CleanArchitecture.Application.Common.Responses;
using CleanArchitecture.Application.Features.Courses.DTOs;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CleanArchitecture.Application.Features.Courses.Queries.Get
{
    public sealed class GetCoursesQueryHandler : IRequestHandler<GetCoursesQuery, Response<List<CourseDto>>>
    {
        private readonly ICourseRepository _courseRepository;
        public GetCoursesQueryHandler(ICourseRepository courseRepository)
        {
            this._courseRepository = courseRepository;
        }

        public async Task<Response<List<CourseDto>>> Handle(GetCoursesQuery request, CancellationToken cancellationToken)
        {
            // Pagination
            request.Pagination.Normalize();

            var (courses, totalCount) = await _courseRepository.GetPagedCoursesAsync(request.Pagination.PageNumber, 
                                                                                                      request.Pagination.PageSize, 
                                                                                                      cancellationToken);

            // Not Found
            if (!courses.Any())
                return ResponseHandler.NotFound<List<CourseDto>>();

            // Success Response
            return ResponseHandler.SuccessPaged(
                courses,
                request.Pagination.PageNumber,
                request.Pagination.PageSize,
                totalCount
            );
        }
    }
}
