using CleanArchitecture.Application.Common.Helpers;
using CleanArchitecture.Application.Common.Interfaces.Repositories;
using CleanArchitecture.Application.Common.Localization;
using CleanArchitecture.Application.Common.Localization.Resources;
using CleanArchitecture.Application.Common.Responses;
using CleanArchitecture.Application.Features.Students.DTOs;
using CleanArchitecture.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace CleanArchitecture.Application.Features.Students.Queries.GetCourses
{
    public sealed class GetCoursesByStudentIdQueryHandler
        : IRequestHandler<GetCoursesByStudentIdQuery, Response<List<StudentCourseDto>>>
    {
        private readonly IStudentRepository _studentRepository;
        private readonly IStringLocalizer<SharedResources> _localizer;

        public GetCoursesByStudentIdQueryHandler(IStudentRepository studentRepository, IStringLocalizer<SharedResources> localizer)
        {
            this._studentRepository = studentRepository;
            this._localizer = localizer;
        }

        public async Task<Response<List<StudentCourseDto>>> Handle(GetCoursesByStudentIdQuery request, CancellationToken cancellationToken)
        {
            // Check Is Student Exists
            var studentExists = await _studentRepository
                .AnyAsync(s => s.Id == request.StudentId, cancellationToken);

            if (!studentExists)
                return ResponseHandler.NotFound<List<StudentCourseDto>>(_localizer[Messages.NotFound, _localizer[Entities.Student]]);


            // Normalize Pagination
            request.Pagination.Normalize();

            var property = GetSortingProperty(request);

            var (courses, totalCount) = await _studentRepository
                .GetCoursesAsync(request.StudentId,
                                 request.Filter?.Search,
                                 property,
                                 request.Sorting.IsDescending,
                                 request.Pagination.PageNumber,
                                 request.Pagination.PageSize,
                                 cancellationToken);

            if(totalCount == 0)
                return ResponseHandler.NotFound<List<StudentCourseDto>>(_localizer[Messages.NoCoursesForStudent]);

            return ResponseHandler.SuccessPaged(
                courses,
                request.Pagination.PageNumber,
                request.Pagination.PageSize,
                totalCount
            );
        }

        private static Expression<Func<StudentCourse, object>> GetSortingProperty(GetCoursesByStudentIdQuery request)
        {
            return request.Sorting?.OrderBy?.Trim().ToLower() switch
            {
                "title" => sc => sc.Course.Title,
                "enrolledat" => sc => sc.EnrolledAt,
                _ => sc => sc.CourseId
            };
        }
    }
}
