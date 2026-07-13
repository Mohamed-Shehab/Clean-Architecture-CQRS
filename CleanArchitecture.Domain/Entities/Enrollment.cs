using CleanArchitecture.Domain.Enums;
using System.ComponentModel.DataAnnotations;

namespace CleanArchitecture.Domain.Entities
{
    public class Enrollment
    {
        [Required]
        public int StudentId { get; set; }

        [Required]
        public int CourseId { get; set; }

        public DateTime EnrolledAt { get; set; }

        public DateTime? CompletedAt { get; set; }

        public DateTime? DroppedAt { get; set; }

        [Required]
        public EnrollmentStatus Status { get; set; } = EnrollmentStatus.Active;


        // Navigation Properties
        public virtual Student Student { get; set; } = null!;

        public virtual Course Course { get; set; } = null!;
    }
}
