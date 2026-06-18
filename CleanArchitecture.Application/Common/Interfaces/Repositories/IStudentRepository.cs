using CleanArchitecture.Application.Features.Students.DTOs;
using CleanArchitecture.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace CleanArchitecture.Application.Common.Interfaces.Repositories
{
    public interface IStudentRepository : IRepository<Student>
    {
        Task<(List<StudentCourseDto> Data, int TotalCount)> GetCoursesAsync(
            int studentId,
            string? search,
            Expression<Func<StudentCourse, object>> property,
            bool descending,
            int pageNumber,
            int pageSize,
            CancellationToken cancellationToken);
    }
}
