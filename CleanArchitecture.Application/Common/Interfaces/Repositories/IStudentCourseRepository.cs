using CleanArchitecture.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CleanArchitecture.Application.Common.Interfaces.Repositories
{
    public interface IStudentCourseRepository : IRepository<StudentCourse>
    {
        Task<bool> IsAssignedAsync(int studentId, int courseId, CancellationToken cancellationToken);

        Task<int> GetStudentsCountAsync(int courseId, CancellationToken cancellationToken);

        Task<StudentCourse?> GetAssignmentAsync(int studentId, int courseId, CancellationToken cancellationToken);
    }
}
