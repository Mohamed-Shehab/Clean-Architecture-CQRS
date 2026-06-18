using Azure.Core;
using CleanArchitecture.Application.Common.Interfaces.Repositories;
using CleanArchitecture.Domain.Entities;
using CleanArchitecture.Infrastructure.Persistence.Context;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CleanArchitecture.Infrastructure.Persistence.Repositories
{
    public class StudentCourseRepository : Repository<StudentCourse>, IStudentCourseRepository
    {
        public StudentCourseRepository(AppDbContext context) : base(context)
        {
        }

        public async Task<bool> IsAssignedAsync(int studentId, int courseId, CancellationToken cancellationToken)
        {
            return await _context.StudentCourses
                .AnyAsync(sc => sc.StudentId == studentId
                                     && sc.CourseId == courseId, cancellationToken);
        }

        public async Task<int> GetStudentsCountAsync(int courseId, CancellationToken cancellationToken)
        {
            return await _context.StudentCourses.
                CountAsync(sc => sc.CourseId == courseId, cancellationToken);
        }

        public async Task<StudentCourse?> GetAssignmentAsync(int studentId, int courseId, CancellationToken cancellationToken)
        {
            return await _context.StudentCourses
                .FirstOrDefaultAsync(sc => sc.StudentId == studentId
                                     && sc.CourseId == courseId, cancellationToken);
        }
        
    }
}
