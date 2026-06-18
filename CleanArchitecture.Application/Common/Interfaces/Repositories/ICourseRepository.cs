using CleanArchitecture.Application.Features.Courses.DTOs;
using CleanArchitecture.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace CleanArchitecture.Application.Common.Interfaces.Repositories
{
    public interface ICourseRepository : IRepository<Course>
    {
        Task<CourseDetailsDto?> GetCourseDetailsAsync(int id, CancellationToken cancellationToken);

        Task<(List<CourseDto> Data, int TotalCount)> GetPagedCoursesAsync(
            int pageNumber,
            int pageSize,
            CancellationToken cancellationToken);

        Task<(List<CourseStudentDto> Data, int TotalCount)> GetStudentsAsync(
            int courseId,
            string? search,
            Expression<Func<Student, object>> property,
            bool descending,
            int pageNumber,
            int pageSize,
            CancellationToken cancellationToken);

    }
}
