namespace CleanArchitecture.Domain.Entities
{
    public class Student
    {

        public int Id { get; set; }

        public DateOnly DateOfBirth { get; set; }

        public string? Address { get; set; }

        public bool IsDeleted { get; set; }

        public DateTime? DeletedAt { get; set; }


        // Foreign Key
        public int UserId { get; set; }


        // Navigation Property
        public virtual ICollection<Enrollment> Enrollments { get; set; } = new HashSet<Enrollment>();

    }
}
