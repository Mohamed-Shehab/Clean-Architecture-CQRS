using CleanArchitecture.Domain.Enums;
using System.ComponentModel.DataAnnotations;

namespace CleanArchitecture.Domain.Entities
{
    public class Enrollment
    {

        public int StudentId { get; set; }

        public int CourseId { get; set; }

        public DateTime EnrolledAt { get; set; }

        public DateTime? CompletedAt { get; set; }

        public DateTime? DroppedAt { get; set; }

        public EnrollmentStatus Status { get; set; } = EnrollmentStatus.Active;


        // Navigation Properties
        public virtual Student Student { get; set; } = null!;

        public virtual Course Course { get; set; } = null!;

    }
}
