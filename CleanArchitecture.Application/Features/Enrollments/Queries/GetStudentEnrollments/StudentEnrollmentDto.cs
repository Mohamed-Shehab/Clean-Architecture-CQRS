using CleanArchitecture.Domain.Enums;

namespace CleanArchitecture.Application.Features.Enrollments.Queries.GetStudentEnrollments
{
    public class StudentEnrollmentDto
    {
        public int CourseId { get; set; }

        public string Name { get; set; } = null!;

        public EnrollmentStatus Status { get; set; }

        public DateTime EnrolledAt { get; set; }

        public DateTime? CompletedAt { get; set; }

        public DateTime? DroppedAt { get; set; }

    }
}
