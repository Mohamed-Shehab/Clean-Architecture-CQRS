using CleanArchitecture.Domain.Enums;

namespace CleanArchitecture.Application.Features.Enrollments.Queries.GetCourseEnrollments
{
    public sealed class CourseEnrollmentDto
    {
        public int StudentId { get; set; }

        public string StudentName { get; set; } = null!;

        public EnrollmentStatus Status { get; set; }

        public DateTime EnrolledAt { get; set; }

        public DateTime? CompletedAt { get; set; }

        public DateTime? DroppedAt { get; set; }
    }
}
