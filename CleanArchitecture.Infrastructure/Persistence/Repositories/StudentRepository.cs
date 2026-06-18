using Azure.Core;
using CleanArchitecture.Application.Common.Interfaces.Repositories;
using CleanArchitecture.Application.Common.Localization;
using CleanArchitecture.Application.Common.Responses;
using CleanArchitecture.Application.Features.Students.DTOs;
using CleanArchitecture.Domain.Entities;
using CleanArchitecture.Infrastructure.Persistence.Context;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace CleanArchitecture.Infrastructure.Persistence.Repositories
{
    public class StudentRepository : Repository<Student>, IStudentRepository
    {
        public StudentRepository(AppDbContext context) : base(context)
        {
        }

        public async Task<(List<StudentCourseDto> Data, int TotalCount)> GetCoursesAsync(int studentId, 
                                                                                         string? search,
                                                                                         Expression<Func<StudentCourse, object>> property, 
                                                                                         bool descending, 
                                                                                         int pageNumber, 
                                                                                         int pageSize, 
                                                                                         CancellationToken cancellationToken)
        {
            // Base Query
            var query = _context.StudentCourses.AsNoTracking()
                .Where(sc => sc.StudentId == studentId);

            // Total Count
            var totalCount = await query.CountAsync(cancellationToken);

            if (totalCount == 0)
                return (new List<StudentCourseDto>(), 0);


            // Filter
            if (!string.IsNullOrWhiteSpace(search))
            {
                search = search.Trim();
                query = query.Where(sc => sc.Course.Title.Contains(search));
            }

            // Sorting
            query = descending
                ? query.OrderByDescending(property)
                : query.OrderBy(property);


            // Pagination + Projection
            var courses = await query
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .Select(sc => new StudentCourseDto
                {
                    CourseId = sc.CourseId,
                    Title = sc.Course.Title,
                    EnrolledAt = sc.EnrolledAt
                })
                .ToListAsync(cancellationToken);

            return (courses, totalCount);
        }
    }
}
