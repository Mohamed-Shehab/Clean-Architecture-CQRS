using CleanArchitecture.Domain.Enums;

namespace CleanArchitecture.Application.Features.Enrollments.Queries.GetStudentEnrollments.Models
{
    public sealed class StudentEnrollmentFilterModel
    {
        public string? Name { get; set; }

        public EnrollmentStatus? Status { get; set; }
    }
}
