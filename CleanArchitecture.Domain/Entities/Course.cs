using System.ComponentModel.DataAnnotations;

namespace CleanArchitecture.Domain.Entities
{
    public class Course
    {
        public Course()
        {
            Enrollments = new List<Enrollment>();
        }

        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(150)]
        public string NameEn { get; set; } = null!;

        [Required]
        [MaxLength(150)]
        public string NameAr { get; set; } = null!;

        [MaxLength(1000)]
        public string? Description { get; set; }

        public int Capacity { get; set; }

        public bool IsActive { get; set; } = true;

        public DateTime CreatedAt { get; set; }

        public DateTime? UpdatedAt { get; set; }


        // Navigation Property
        public virtual ICollection<Enrollment> Enrollments { get; set; }
    }
}
