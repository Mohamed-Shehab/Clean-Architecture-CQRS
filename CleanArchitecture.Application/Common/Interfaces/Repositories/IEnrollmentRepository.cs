using CleanArchitecture.Application.Common.Models.Querying;
using CleanArchitecture.Application.Features.Enrollments.Queries.GetCourseEnrollments;
using CleanArchitecture.Application.Features.Enrollments.Queries.GetCourseEnrollments.Models;
using CleanArchitecture.Application.Features.Enrollments.Queries.GetStudentEnrollments;
using CleanArchitecture.Application.Features.Enrollments.Queries.GetStudentEnrollments.Models;
using CleanArchitecture.Domain.Entities;

namespace CleanArchitecture.Application.Common.Interfaces.Repositories
{
    public interface IEnrollmentRepository : IRepository<Enrollment>
    {
        Task<List<Enrollment>> GetStudentActiveEnrollmentsAsync(
            int studentId, 
            CancellationToken cancellationToken);


        Task<Enrollment?> GetEnrollmentAsync(
            int studentId, 
            int courseId, 
            CancellationToken cancellationToken);


        Task<(List<StudentEnrollmentDto> Data, int TotalCount)> GetStudentEnrollmentsAsync(
            int studentId,
            StudentEnrollmentFilterModel? filter,
            StudentEnrollmentSortingModel? sorting,
            PaginationModel pagination,
            CancellationToken cancellationToken);


        Task<(List<CourseEnrollmentDto> Data, int TotalCount)> GetCourseEnrollmentsAsync(
            int courseId,
            CourseEnrollmentFilterModel? filter,
            CourseEnrollmentSortingModel? sorting,
            PaginationModel pagination,
            CancellationToken cancellationToken);


    }
}
