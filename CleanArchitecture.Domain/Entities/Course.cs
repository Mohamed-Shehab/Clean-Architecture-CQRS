namespace CleanArchitecture.Domain.Entities
{
    public class Course
    {

        public int Id { get; set; }

        public string NameEn { get; set; } = null!;

        public string NameAr { get; set; } = null!;

        public string? Description { get; set; }

        public int Capacity { get; set; }

        public bool IsActive { get; set; }

        public bool IsDeleted { get; set; }

        public DateTime CreatedAt { get; set; }

        public DateTime? UpdatedAt { get; set; }

        public DateTime? DeletedAt { get; set; }


        // Navigation Property
        public virtual ICollection<Enrollment> Enrollments { get; set; } = new HashSet<Enrollment>();

    }
}
