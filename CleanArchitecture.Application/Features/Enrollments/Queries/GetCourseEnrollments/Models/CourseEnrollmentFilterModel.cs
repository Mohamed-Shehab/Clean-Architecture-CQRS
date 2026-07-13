using CleanArchitecture.Domain.Enums;

namespace CleanArchitecture.Application.Features.Enrollments.Queries.GetCourseEnrollments.Models
{
    public sealed class CourseEnrollmentFilterModel
    {
        public string? StudentName { get; set; }

        public string? Email { get; set; }

        public string? PhoneNumber { get; set; }

        public EnrollmentStatus? Status { get; set; }
    }
}
