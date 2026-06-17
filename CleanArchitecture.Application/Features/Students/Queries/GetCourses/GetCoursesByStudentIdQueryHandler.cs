using CleanArchitecture.Application.Common.Helpers;
using CleanArchitecture.Application.Common.Localization;
using CleanArchitecture.Application.Common.Localization.Resources;
using CleanArchitecture.Application.Common.Responses;
using CleanArchitecture.Application.Features.Students.DTOs;
using CleanArchitecture.Domain.Entities;
using CleanArchitecture.Infrastructure.Persistence.Context;
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
        private readonly AppDbContext _context;
        private readonly IStringLocalizer<SharedResources> _localizer;

        public GetCoursesByStudentIdQueryHandler(AppDbContext context, IStringLocalizer<SharedResources> localizer)
        {
            this._context = context;
            this._localizer = localizer;
        }

        public async Task<Response<List<StudentCourseDto>>> Handle(GetCoursesByStudentIdQuery request, CancellationToken cancellationToken)
        {
            // Check Is Student Exists
            var studentExists = await _context.Students
                .AnyAsync(s => s.Id == request.StudentId, cancellationToken);

            if (!studentExists)
                return ResponseHandler.NotFound<List<StudentCourseDto>>(_localizer[Messages.NotFound, _localizer[Entities.Student]]);


            // Normalize Pagination
            request.Pagination.Normalize();


            // Base Query
            var query = _context.StudentCourses.AsNoTracking()
                .Where(sc => sc.StudentId == request.StudentId);

            // Total Count
            var totalCount = await query.CountAsync(cancellationToken);

            if (totalCount == 0)
                return ResponseHandler.NotFound<List<StudentCourseDto>>(_localizer[Messages.NoCoursesForStudent]);


            // Filter
            if (!string.IsNullOrWhiteSpace(request.Filter?.Search))
            {
                var search = request.Filter.Search.Trim();
                query = query.Where(sc => sc.Course.Title.Contains(search));
            }

            // Sorting
            if (request.Sorting != null)
            {
                query = request.Sorting.IsDescending
                    ? query.OrderByDescending(GetSortingProperty(request))
                    : query.OrderBy(GetSortingProperty(request));
            }


            // Pagination + Projection
            var courses = await query
                .Skip((request.Pagination.PageNumber - 1) * request.Pagination.PageSize)
                .Take(request.Pagination.PageSize)
                .Select(sc => new StudentCourseDto
                {
                    CourseId = sc.CourseId,
                    Title = sc.Course.Title,
                    EnrolledAt = sc.EnrolledAt
                })
                .ToListAsync(cancellationToken);


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
