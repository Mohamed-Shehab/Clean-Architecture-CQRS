using System.ComponentModel.DataAnnotations;

namespace CleanArchitecture.Domain.Entities
{
    public class Student
    {
        public Student()
        {
            Enrollments = new List<Enrollment>();
        }

        [Key]
        public int Id { get; set; }

        [Required]
        public DateOnly DateOfBirth { get; set; }

        [MaxLength(250)]
        public string? Address { get; set; }


        // Foreign Key
        [Required]
        public int UserId { get; set; }


        // Navigation Property
        public virtual ICollection<Enrollment> Enrollments { get; set; }
    }
}
