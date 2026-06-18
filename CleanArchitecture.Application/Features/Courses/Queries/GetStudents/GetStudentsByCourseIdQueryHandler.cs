using CleanArchitecture.Application.Common.Helpers;
using CleanArchitecture.Application.Common.Interfaces.Repositories;
using CleanArchitecture.Application.Common.Responses;
using CleanArchitecture.Application.Features.Courses.DTOs;
using CleanArchitecture.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace CleanArchitecture.Application.Features.Courses.Queries.GetStudents
{
    public class GetStudentsByCourseIdQueryHandler : IRequestHandler<GetStudentsByCourseIdQuery, Response<List<CourseStudentDto>>>
    {
        private readonly ICourseRepository _courseRepository;

        public GetStudentsByCourseIdQueryHandler(ICourseRepository courseRepository)
        {
            this._courseRepository = courseRepository;
        }

        public async Task<Response<List<CourseStudentDto>>> Handle(GetStudentsByCourseIdQuery request, CancellationToken cancellationToken)
        {
            // Normalize Pagination
            request.Pagination.Normalize();

            var property = GetSortingProperty(request);

            var (students, totalCount) = await _courseRepository.GetStudentsAsync(
                request.Id,
                request.Filter?.Search,
                property,
                request.Sorting.IsDescending,
                request.Pagination.PageNumber,
                request.Pagination.PageSize,
                cancellationToken
            );
            

            if (!students.Any())
                return ResponseHandler.NotFound<List<CourseStudentDto>>();

            return ResponseHandler.SuccessPaged(
                students,
                request.Pagination.PageNumber,
                request.Pagination.PageSize,
                totalCount
            );
        }

        private static Expression<Func<Student, object>> GetSortingProperty(GetStudentsByCourseIdQuery request)
        {

            return request.Sorting?.OrderBy?.Trim().ToLower() switch
            {
                "name" => s => s.Name,

                "email" => s => s.Email,

                _ => s => s.Id
            };
        }
    }
}
