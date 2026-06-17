using CleanArchitecture.Application.Common.Helpers;
using CleanArchitecture.Application.Common.Responses;
using CleanArchitecture.Application.Features.Courses.DTOs;
using CleanArchitecture.Domain.Entities;
using CleanArchitecture.Infrastructure.Persistence.Context;
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
        private readonly AppDbContext _context;

        public GetStudentsByCourseIdQueryHandler(AppDbContext context)
        {
            this._context = context;
        }

        public async Task<Response<List<CourseStudentDto>>> Handle(GetStudentsByCourseIdQuery request, CancellationToken cancellationToken)
        {
            // Normalize Pagination
            request.Pagination.Normalize();


            // Build Base Query
            var query = _context.StudentCourses.AsNoTracking()
                .Where(sc => sc.CourseId == request.Id)
                .Select(sc => sc.Student);


            // Filtration
            Expression<Func<Student, bool>>? filter = null;
            if (!string.IsNullOrWhiteSpace(request.Filter?.Search))
            {
                var search = request.Filter.Search.Trim();

                filter = s => s.Name.Contains(search) || s.Email.Contains(search);
            }

            if (filter != null)
                query = query.Where(filter);


            // Total Count
            var totalCount = await query.CountAsync(cancellationToken);


            // Sorting
            Func<IQueryable<Student>, IOrderedQueryable<Student>>? orderBy = null;
            if (request.Sorting != null)
            {
                orderBy = request.Sorting.IsDescending
                    ? q => q.OrderByDescending(GetSortingProperty(request))
                    : q => q.OrderBy(GetSortingProperty(request));
            }

            if (orderBy != null)
                query = orderBy(query);


            // Apply Pagination
            query = query
                .Skip((request.Pagination.PageNumber - 1) * request.Pagination.PageSize)
                .Take(request.Pagination.PageSize);

            // Projection
            var students = await query
                .Select(s => new CourseStudentDto
                {
                    Id = s.Id,
                    Name = s.Name,
                    Email = s.Email
                })
                .ToListAsync(cancellationToken);

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
