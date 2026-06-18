using Azure.Core;
using CleanArchitecture.Application.Common.Interfaces.Repositories;
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

namespace CleanArchitecture.Infrastructure.Persistence.Repositories
{
    public class CourseRepository : Repository<Course>, ICourseRepository
    {
        public CourseRepository(AppDbContext context) : base(context)
        {
        }

        public async Task<CourseDetailsDto?> GetCourseDetailsAsync(int courseId, CancellationToken cancellationToken)
        {
            return await _context.Courses
                .Where(c => c.Id == courseId)
                .Select(c => new CourseDetailsDto
                {
                    Id = c.Id,
                    Name = c.Title,
                    CourseCapacity = c.MaxStudents
                })
                .FirstOrDefaultAsync(cancellationToken);
        }

        public async Task<(List<CourseDto> Data, int TotalCount)> GetPagedCoursesAsync(int pageNumber, 
                                                                                       int pageSize, 
                                                                                       CancellationToken cancellationToken)
        {
            // Base query
            var query = _context.Courses.AsQueryable();

            // Total count Before pagination
            var totalCount = await query.CountAsync(cancellationToken);

            // Get paged data with projection
            var courses = await query
                .OrderBy(c => c.Id)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .Select(c => new CourseDto
                {
                    Id = c.Id,
                    Name = c.Title,
                    StudentsCount = c.StudentCourses.Count()
                })
                .ToListAsync(cancellationToken);
            return (courses, totalCount);
        }

        public async Task<(List<CourseStudentDto> Data, int TotalCount)> GetStudentsAsync(int courseId, 
                                                                                    string? search,
                                                                                    Expression<Func<Student, object>> property, 
                                                                                    bool descending, 
                                                                                    int pageNumber, 
                                                                                    int pageSize, 
                                                                                    CancellationToken cancellationToken)
        {
            // Build Base Query
            var query = _context.StudentCourses.AsNoTracking()
                .Where(sc => sc.CourseId == courseId)
                .Select(sc => sc.Student);


            // Filtration
            Expression<Func<Student, bool>>? filter = null;
            if (!string.IsNullOrWhiteSpace(search))
            {
                search = search.Trim();

                filter = s => s.Name.Contains(search) || s.Email.Contains(search);
            }

            if (filter != null)
                query = query.Where(filter);


            // Total Count
            var totalCount = await query.CountAsync(cancellationToken);


            // Sorting
            Func<IQueryable<Student>, IOrderedQueryable<Student>>? orderBy = null;
            
            orderBy = descending
                ? q => q.OrderByDescending(property)
                : q => q.OrderBy(property);

            if (orderBy != null)
                query = orderBy(query);


            // Apply Pagination
            query = query
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize);

            // Projection
            var students = await query
                .Select(s => new CourseStudentDto
                {
                    Id = s.Id,
                    Name = s.Name,
                    Email = s.Email
                })
                .ToListAsync(cancellationToken);

            return (students, totalCount);
        }

    }
}
